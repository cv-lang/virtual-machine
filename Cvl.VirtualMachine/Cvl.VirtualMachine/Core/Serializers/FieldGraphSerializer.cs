using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cvl.VirtualMachine.Core.Serializers
{
    /// <summary>
    /// Zapisuje cały graf obiektów pole po polu (także prywatne), z zachowaniem współdzielonych referencji.
    /// Wynik to JSON bezpieczny dla kolumny jsonb w PostgreSQL: bez znaku \u0000 i niezależny od kolejności kluczy.
    /// Stan to dane zaufane - odczyt tworzy obiekty bez wywoływania konstruktorów.
    /// Pola [NonSerialized] nie wchodzą do stanu, odtwarza je IDeserializedObject.AfterDeserialization.
    /// </summary>
    public class FieldGraphSerializer : ISerializer
    {
        /// <summary>
        /// Podnosić przy każdej zmianie, po której stare stany nie dałyby się poprawnie odczytać
        /// </summary>
        public const int FormatVersion = 1;

        internal const int MaxDepth = 512;

        public string Serialize(object? obj)
        {
            return new GraphWriter().Write(obj);
        }

        public T? Deserialize<T>(string serializedForm)
        {
            return (T?)new GraphReader().Read(serializedForm);
        }
    }

    internal class GraphWriter
    {
        private static readonly JsonSerializerOptions options = new()
        {
            //polskie znaki zostają czytelne, stan nie trafia do HTML
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            MaxDepth = FieldGraphSerializer.MaxDepth,
        };

        private readonly Dictionary<object, int> ids = new(ReferenceEqualityComparer.Instance);
        private readonly Queue<(object Value, int Id, string Path)> pending = new();

        public string Write(object? root)
        {
            var document = new JsonObject
            {
                ["format"] = FieldGraphSerializer.FormatVersion,
                ["root"] = WriteValue(root, typeof(object), "proces"),
            };

            var objects = new JsonArray();
            while (pending.Count > 0)
            {
                var (value, id, path) = pending.Dequeue();
                objects.Add(WriteObject(value, id, path));
            }
            document["objects"] = objects;

            return document.ToJsonString(options);
        }

        /// <summary>
        /// null, tekst, int i bool to zwykłe wartości JSON, cała reszta to obiekt z rodzajem wartości lub referencją
        /// </summary>
        private JsonNode? WriteValue(object? value, Type declaredType, string path)
        {
            switch (value)
            {
                case null:
                    return null;
                case string text:
                    return WriteString(text);
                case int number:
                    return JsonValue.Create(number);
                case bool flag:
                    return JsonValue.Create(flag);
            }

            var type = value.GetType();
            if (type.IsEnum)
            {
                var number = Convert.ChangeType(value, Enum.GetUnderlyingType(type), CultureInfo.InvariantCulture);
                return new JsonObject
                {
                    ["t"] = "enum",
                    ["c"] = TypeNames.Get(type),
                    ["v"] = Convert.ToString(number, CultureInfo.InvariantCulture),
                };
            }

            if (SimpleValues.TryEncode(value, out var code, out var encoded))
            {
                return new JsonObject { ["t"] = code, ["v"] = encoded };
            }

            if (type.IsValueType && declaredType.IsValueType)
            {
                //struktura trzymana w miejscu - wartość bez tożsamości
                var slot = new JsonObject { ["t"] = "struct", ["c"] = TypeNames.Get(type) };
                WriteFields(slot, value, type, path);
                return slot;
            }

            //obiekt lub struktura w pudełku (box) - pudełko może być współdzielone, np. przez adres pola
            if (ids.TryGetValue(value, out var id) == false)
            {
                id = ids.Count + 1;
                ids.Add(value, id);
                pending.Enqueue((value, id, path));
            }
            return new JsonObject { ["r"] = id };
        }

        private static JsonNode WriteString(string text)
        {
            //\u0000 odrzuca jsonb, a niesparowanych surogatów nie da się zapisać w UTF-8 - oba idą przez base64
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsHighSurrogate(text[i]) && i + 1 < text.Length && char.IsLowSurrogate(text[i + 1]))
                {
                    i++;
                }
                else if (text[i] == '\0' || char.IsSurrogate(text[i]))
                {
                    return new JsonObject { ["t"] = "s64", ["v"] = Convert.ToBase64String(MemoryMarshal.AsBytes(text.AsSpan())) };
                }
            }

            return JsonValue.Create(text);
        }

        private JsonObject WriteObject(object value, int id, string path)
        {
            var type = value.GetType();
            var element = new JsonObject { ["id"] = id, ["c"] = TypeNames.Get(type) };

            if (value is Array array)
            {
                WriteArray(element, array, path);
            }
            else if (value is Delegate del)
            {
                WriteDelegate(element, del, path);
            }
            else if (SerializableTypes.IsDictionary(type))
            {
                WriteDictionary(element, value, type, path);
            }
            else if (SerializableTypes.IsHashSet(type))
            {
                WriteHashSet(element, value, type, path);
            }
            else
            {
                WriteFields(element, value, type, path);
            }

            return element;
        }

        private void WriteFields(JsonObject parent, object value, Type type, string path)
        {
            SerializableTypes.EnsureAllowed(type, path);
            SerializableTypes.EnsureEnumeratorCanBeRestored(value, type, path);

            var fields = new JsonObject();
            foreach (var (name, field) in SerializableTypes.GetFields(type))
            {
                var fieldValue = field.GetValue(value);
                if (fieldValue == null && value is Exception exception && field.Name == "_stackTraceString")
                {
                    fieldValue = exception.StackTrace;
                }

                if (fieldValue != null)
                {
                    fields[name] = WriteValue(fieldValue, field.FieldType, $"{path}.{name}");
                }
            }

            if (fields.Count > 0)
            {
                parent["f"] = fields;
            }
        }

        private void WriteArray(JsonObject element, Array array, string path)
        {
            if (array.Rank != 1)
            {
                throw new StateSerializationException($"Tablice wielowymiarowe nie mogą być częścią stanu procesu ({path})");
            }

            element["len"] = array.Length;

            if (array is byte[] bytes)
            {
                element["b64"] = Convert.ToBase64String(bytes);
                return;
            }

            //tablice list mają zapas pojemności, więc zapisujemy tylko niepuste elementy
            var elementType = array.GetType().GetElementType()!;
            var items = new JsonObject();
            for (int i = 0; i < array.Length; i++)
            {
                var item = array.GetValue(i);
                if (item != null)
                {
                    items[i.ToString(CultureInfo.InvariantCulture)] = WriteValue(item, elementType, $"{path}[{i}]");
                }
            }
            element["i"] = items;
        }

        private void WriteDelegate(JsonObject element, Delegate del, string path)
        {
            var list = del.GetInvocationList();
            if (list.Length > 1)
            {
                var parts = new JsonArray();
                for (int i = 0; i < list.Length; i++)
                {
                    parts.Add(WriteValue(list[i], typeof(Delegate), $"{path}[{i}]"));
                }
                element["parts"] = parts;
                return;
            }

            element["m"] = MemberIdentity.Get(del.Method);
            element["target"] = WriteValue(del.Target, typeof(object), $"{path}.Target");
        }

        private void WriteDictionary(JsonObject element, object value, Type type, string path)
        {
            WriteHashCollectionHeader(element, value, type, path);

            var args = type.GetGenericArguments();
            var entries = new JsonArray();
            foreach (DictionaryEntry entry in (IDictionary)value)
            {
                entries.Add(new JsonArray(
                    WriteValue(entry.Key, args[0], $"{path}[klucz]"),
                    WriteValue(entry.Value, args[1], $"{path}[{entry.Key}]")));
            }
            element["e"] = entries;
        }

        private void WriteHashSet(JsonObject element, object value, Type type, string path)
        {
            WriteHashCollectionHeader(element, value, type, path);

            var itemType = type.GetGenericArguments()[0];
            var entries = new JsonArray();
            foreach (var item in (IEnumerable)value)
            {
                entries.Add(WriteValue(item, itemType, $"{path}[element]"));
            }
            element["e"] = entries;
        }

        private void WriteHashCollectionHeader(JsonObject element, object value, Type type, string path)
        {
            var version = type.GetField("_version", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(value);
            if (version != null)
            {
                element["ver"] = (int)version;
            }

            var comparer = type.GetProperty("Comparer")!.GetValue(value);
            var defaultComparer = typeof(EqualityComparer<>).MakeGenericType(type.GetGenericArguments()[0])
                .GetProperty("Default")!.GetValue(null);
            if (Equals(comparer, defaultComparer) == false)
            {
                element["comparer"] = WriteValue(comparer, typeof(object), $"{path}.Comparer");
            }
        }
    }

    internal class GraphReader
    {
        private readonly Dictionary<int, JsonObject> elements = new();
        private readonly Dictionary<int, object> objects = new();
        private readonly HashSet<int> filled = new();
        private readonly List<int> hashCollections = new();

        public object? Read(string serialized)
        {
            var document = Parse(serialized);

            var format = document["format"]?.GetValue<int>();
            if (format != FieldGraphSerializer.FormatVersion)
            {
                throw new StateSerializationException(
                    $"Nieobsługiwany format stanu: {format?.ToString() ?? "brak"}, oczekiwany {FieldGraphSerializer.FormatVersion}");
            }

            foreach (var node in document["objects"]?.AsArray() ?? new JsonArray())
            {
                var element = node!.AsObject();
                elements.Add(element["id"]!.GetValue<int>(), element);
            }

            var root = ReadValue(document["root"]);

            var orderedIds = elements.Keys.OrderBy(id => id).ToList();
            foreach (var id in orderedIds)
            {
                Fill(id);
            }

            //dopiero teraz klucze mają wypełnione pola, od których zależą ich hasze
            foreach (var id in hashCollections)
            {
                FillHashCollection(id);
            }

            foreach (var id in orderedIds)
            {
                if (objects[id] is IDeserializedObject deserialized)
                {
                    deserialized.AfterDeserialization();
                }
            }

            return root;
        }

        private static JsonObject Parse(string serialized)
        {
            try
            {
                var options = new JsonDocumentOptions { MaxDepth = FieldGraphSerializer.MaxDepth };
                return JsonNode.Parse(serialized, null, options) as JsonObject
                       ?? throw new StateSerializationException("Nieobsługiwany format stanu: dokument JSON nie jest obiektem");
            }
            catch (JsonException ex)
            {
                throw new StateSerializationException("Nieobsługiwany format stanu: to nie jest poprawny JSON", ex);
            }
        }

        private object GetObject(int id)
        {
            if (objects.TryGetValue(id, out var existing))
            {
                return existing;
            }

            if (elements.TryGetValue(id, out var element) == false)
            {
                throw new StateSerializationException($"Stan odwołuje się do nieistniejącego obiektu {id}");
            }

            var type = TypeNames.Resolve(element["c"]!.GetValue<string>());
            object created;

            if (type.IsArray)
            {
                var bytes = element["b64"];
                created = bytes != null
                    ? Convert.FromBase64String(bytes.GetValue<string>())
                    : Array.CreateInstance(type.GetElementType()!, element["len"]!.GetValue<int>());
                objects.Add(id, created);
            }
            else if (typeof(Delegate).IsAssignableFrom(type))
            {
                created = CreateDelegate(type, element);
                objects.Add(id, created);
                filled.Add(id);
            }
            else if (SerializableTypes.IsDictionary(type) || SerializableTypes.IsHashSet(type))
            {
                created = CreateHashCollection(type, element);
                objects.Add(id, created);
                hashCollections.Add(id);
                filled.Add(id);
            }
            else
            {
                SerializableTypes.EnsureAllowed(type, $"obiekt {id}");
                created = RuntimeHelpers.GetUninitializedObject(type);
                objects.Add(id, created);
            }

            return created;
        }

        private void Fill(int id)
        {
            var target = GetObject(id);
            if (filled.Add(id) == false)
            {
                return;
            }

            var element = elements[id];
            if (target is Array array)
            {
                foreach (var (index, slot) in element["i"]?.AsObject() ?? new JsonObject())
                {
                    array.SetValue(ReadValue(slot), int.Parse(index, CultureInfo.InvariantCulture));
                }
                return;
            }

            FillFields(target, target.GetType(), element);
        }

        private void FillFields(object target, Type type, JsonObject parent)
        {
            var fields = SerializableTypes.GetFields(type);
            foreach (var (name, slot) in parent["f"]?.AsObject() ?? new JsonObject())
            {
                if (fields.TryGetValue(name, out var field) == false)
                {
                    throw new StateSerializationException(
                        $"Stan zawiera pole {type.FullName}.{name}, którego nie ma w załadowanym kodzie");
                }
                field.SetValue(target, ReadValue(slot));
            }
        }

        private object? ReadValue(JsonNode? slot)
        {
            switch (slot)
            {
                case null:
                    return null;
                case JsonValue value:
                    return value.GetValueKind() switch
                    {
                        JsonValueKind.String => value.GetValue<string>(),
                        JsonValueKind.Number => value.GetValue<int>(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        _ => throw new StateSerializationException($"Nieobsługiwana wartość w stanie: {value.ToJsonString()}"),
                    };
            }

            var element = slot.AsObject();
            var reference = element["r"];
            if (reference != null)
            {
                return GetObject(reference.GetValue<int>());
            }

            var code = element["t"]!.GetValue<string>();
            switch (code)
            {
                case "s64":
                    return new string(MemoryMarshal.Cast<byte, char>(Convert.FromBase64String(element["v"]!.GetValue<string>())));
                case "enum":
                    var enumType = TypeNames.Resolve(element["c"]!.GetValue<string>());
                    var number = Convert.ChangeType(element["v"]!.GetValue<string>(), Enum.GetUnderlyingType(enumType), CultureInfo.InvariantCulture);
                    return Enum.ToObject(enumType, number);
                case "struct":
                    var structType = TypeNames.Resolve(element["c"]!.GetValue<string>());
                    SerializableTypes.EnsureAllowed(structType, "struktura w stanie");
                    var box = RuntimeHelpers.GetUninitializedObject(structType);
                    FillFields(box, structType, element);
                    return box;
                default:
                    return SimpleValues.Decode(code, element["v"]!.GetValue<string>());
            }
        }

        private Delegate CreateDelegate(Type type, JsonObject element)
        {
            var parts = element["parts"];
            if (parts != null)
            {
                var list = parts.AsArray().Select(p => (Delegate)ReadValue(p)!).ToArray();
                return Delegate.Combine(list)!;
            }

            var method = (MethodInfo)MemberIdentity.ResolveMethod(element["m"]!.GetValue<string>());
            return Delegate.CreateDelegate(type, ReadValue(element["target"]), method);
        }

        private object CreateHashCollection(Type type, JsonObject element)
        {
            var comparerSlot = element["comparer"];
            if (comparerSlot == null)
            {
                return Activator.CreateInstance(type)!;
            }

            //kolekcja dostaje porównywacz w konstruktorze, więc musi być już wypełniony
            var comparer = ReadValue(comparerSlot);
            var reference = comparerSlot["r"];
            if (reference != null)
            {
                Fill(reference.GetValue<int>());
            }

            var comparerType = typeof(IEqualityComparer<>).MakeGenericType(type.GetGenericArguments()[0]);
            return type.GetConstructor(new[] { comparerType })!.Invoke(new[] { comparer });
        }

        private void FillHashCollection(int id)
        {
            var target = objects[id];
            var type = target.GetType();
            var element = elements[id];
            var entries = element["e"]?.AsArray() ?? new JsonArray();

            if (target is IDictionary dictionary)
            {
                foreach (var entry in entries)
                {
                    dictionary.Add(ReadValue(entry![0])!, ReadValue(entry[1]));
                }
            }
            else
            {
                var add = type.GetMethod("Add")!;
                foreach (var entry in entries)
                {
                    add.Invoke(target, new[] { ReadValue(entry) });
                }
            }

            //zgodność z enumeratorem zapisanym w trakcie foreach
            var version = element["ver"];
            if (version != null)
            {
                type.GetField("_version", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(target, version.GetValue<int>());
            }
        }
    }
}
