using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cvl.VirtualMachine.Core.Serializers
{
    /// <summary>
    /// Błąd zapisu lub odczytu stanu maszyny - zawsze jawny, stan nigdy nie jest gubiony po cichu
    /// </summary>
    public class StateSerializationException : Exception
    {
        public StateSerializationException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Nazwy typów bez wersji assembly - stan przeżywa zmianę wersji bibliotek
    /// </summary>
    internal static class TypeNames
    {
        private static readonly ConcurrentDictionary<Type, string> names = new();
        private static readonly ConcurrentDictionary<string, Type?> types = new();

        public static string Get(Type type)
        {
            return names.GetOrAdd(type, t => $"{NameWithoutAssembly(t)}, {RootAssembly(t).GetName().Name}");
        }

        public static Type Resolve(string name)
        {
            var type = types.GetOrAdd(name, n => Type.GetType(n, false));
            if (type == null)
            {
                throw new StateSerializationException($"Stan zawiera typ, którego nie ma w załadowanym kodzie: {name}");
            }
            return type;
        }

        private static string NameWithoutAssembly(Type t)
        {
            if (t.IsArray)
            {
                return $"{NameWithoutAssembly(t.GetElementType()!)}[{new string(',', t.GetArrayRank() - 1)}]";
            }
            if (t.IsByRef)
            {
                return NameWithoutAssembly(t.GetElementType()!) + "&";
            }
            if (t.IsGenericType && t.IsGenericTypeDefinition == false)
            {
                var args = t.GetGenericArguments().Select(a => $"[{Get(a)}]");
                return $"{t.GetGenericTypeDefinition().FullName}[{string.Join(",", args)}]";
            }
            return t.FullName ?? t.Name;
        }

        private static Assembly RootAssembly(Type t)
        {
            return t.HasElementType ? RootAssembly(t.GetElementType()!) : t.Assembly;
        }
    }

    /// <summary>
    /// Tożsamość metody i pola: typ, nazwa i pełna sygnatura.
    /// Sama nazwa nie wystarcza - przeciążenia i metody prywatne muszą się odtworzyć jednoznacznie.
    /// </summary>
    internal static class MemberIdentity
    {
        private const BindingFlags AllMembers = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public
                                                | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        private static readonly ConcurrentDictionary<string, MethodBase> methods = new();

        public static string Get(MethodBase method)
        {
            var genericArgs = method.IsGenericMethod
                ? method.GetGenericArguments().Select(TypeNames.Get)
                : Enumerable.Empty<string>();
            var parameters = method.GetParameters().Select(p => TypeNames.Get(p.ParameterType));
            var owner = method.DeclaringType != null ? TypeNames.Get(method.DeclaringType) : method.Module.Name;
            return $"{owner}|{method.Name}|{string.Join(";", genericArgs)}|{string.Join(";", parameters)}";
        }

        public static string Get(FieldInfo field)
        {
            var owner = field.DeclaringType != null ? TypeNames.Get(field.DeclaringType) : field.Module.Name;
            return $"{owner}|{field.Name}";
        }

        public static MethodBase ResolveMethod(string identity)
        {
            return methods.GetOrAdd(identity, FindMethod);
        }

        public static FieldInfo ResolveField(string identity)
        {
            var parts = identity.Split('|');
            return TypeNames.Resolve(parts[0]).GetField(parts[1], AllMembers)
                   ?? throw new StateSerializationException($"Stan odwołuje się do pola, którego nie ma w kodzie: {identity}");
        }

        private static MethodBase FindMethod(string identity)
        {
            var parts = identity.Split('|');
            var type = TypeNames.Resolve(parts[0]);
            var name = parts[1];
            var genericArgs = parts[2].Length == 0 ? Array.Empty<Type>() : parts[2].Split(';').Select(TypeNames.Resolve).ToArray();

            var candidates = name is ".ctor" or ".cctor"
                ? type.GetConstructors(AllMembers).Cast<MethodBase>()
                : type.GetMethods(AllMembers).Where(m => m.Name == name).Cast<MethodBase>();

            foreach (var candidate in candidates)
            {
                var closed = candidate;
                if (genericArgs.Length > 0)
                {
                    if (candidate is not MethodInfo { IsGenericMethodDefinition: true } generic
                        || generic.GetGenericArguments().Length != genericArgs.Length)
                    {
                        continue;
                    }
                    closed = generic.MakeGenericMethod(genericArgs);
                }

                if (Get(closed) == identity)
                {
                    return closed;
                }
            }

            throw new StateSerializationException($"Stan odwołuje się do metody, której nie ma w kodzie: {identity}");
        }
    }

    /// <summary>
    /// Pola, które wchodzą do stanu, i typy systemowe, które wolno zapisywać.
    /// Typy spoza listy (Task, Stream, HttpClient, DbContext...) trzymają zasoby, których nie da się odtworzyć,
    /// dlatego zapis kończy się czytelnym błędem. Ta sama lista chroni odczyt przed tworzeniem dowolnych typów systemowych.
    /// </summary>
    internal static class SerializableTypes
    {
        private static readonly string[] allowedNamespaces =
        {
            "System.Collections.Generic",
            "System.Collections.ObjectModel",
            "System.Linq",
        };

        private static readonly Type[] allowedTypes =
        {
            typeof(object),
            typeof(Nullable<>),
            typeof(StringBuilder),
            typeof(Version),
            typeof(DateOnly),
            typeof(TimeOnly),
        };

        //System.Exception trzyma też wskaźniki do kodu natywnego, stos wywołań zostaje jako tekst
        private static readonly HashSet<string> exceptionFields = new()
        {
            "_message", "_innerException", "_source", "_HResult", "_remoteStackTraceString", "_stackTraceString",
        };

        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, FieldInfo>> fields = new();

        public static bool IsAllowed(Type type)
        {
            var assemblyName = type.Assembly.GetName().Name ?? "";
            var isSystem = assemblyName.StartsWith("System.") || assemblyName is "System" or "mscorlib" or "netstandard";
            if (isSystem == false)
            {
                return true;
            }

            if (typeof(Delegate).IsAssignableFrom(type) || typeof(Exception).IsAssignableFrom(type))
            {
                return true;
            }

            if (allowedNamespaces.Contains(type.Namespace))
            {
                return true;
            }

            var definition = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            return allowedTypes.Contains(definition)
                   || definition.FullName!.StartsWith("System.Tuple`")
                   || definition.FullName!.StartsWith("System.ValueTuple`")
                   || definition.FullName!.StartsWith("System.SZGenericArrayEnumerator");
        }

        public static void EnsureAllowed(Type type, string where)
        {
            if (IsAllowed(type) == false)
            {
                throw new StateSerializationException(
                    $"Typ {type.FullName} nie może być częścią stanu procesu ({where}). "
                    + "Oznacz pole [NonSerialized] i odtwórz je w IDeserializedObject.AfterDeserialization albo nie trzymaj go przez hibernację.");
            }
        }

        public static IReadOnlyDictionary<string, FieldInfo> GetFields(Type type)
        {
            return fields.GetOrAdd(type, CollectFields);
        }

        public static bool IsDictionary(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        public static bool IsHashSet(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>);
        }

        /// <summary>
        /// Słowniki i zbiory zapisujemy jako listę elementów - hasze stringów są losowe per proces.
        /// Enumerator trzyma indeks wewnętrznej tablicy, a po usunięciach elementów odbudowa ją zagęszcza,
        /// więc foreach przez hibernację pominąłby elementy. Taki stan odrzucamy zamiast go uszkodzić.
        /// </summary>
        public static void EnsureEnumeratorCanBeRestored(object value, Type type, string where)
        {
            var name = type.FullName ?? "";
            if (type.Name.Contains("Enumerator") == false
                || (name.StartsWith("System.Collections.Generic.Dictionary`2+") == false
                    && name.StartsWith("System.Collections.Generic.HashSet`1+") == false))
            {
                return;
            }

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var collection = (type.GetField("_dictionary", flags) ?? type.GetField("_hashSet", flags))?.GetValue(value);
            var freeCount = collection?.GetType().GetField("_freeCount", flags)?.GetValue(collection);
            if (freeCount is int and > 0)
            {
                throw new StateSerializationException(
                    $"Nie można zapisać foreach po słowniku lub zbiorze, z którego wcześniej usunięto elementy ({where}). "
                    + "Przejdź po kopii, np. ToList().");
            }
        }

        private static IReadOnlyDictionary<string, FieldInfo> CollectFields(Type type)
        {
            var result = new Dictionary<string, FieldInfo>();
            for (var current = type; current != null && current != typeof(object); current = current.BaseType)
            {
                var declared = current.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                foreach (var field in declared)
                {
                    if (field.IsNotSerialized || (current == typeof(Exception) && exceptionFields.Contains(field.Name) == false))
                    {
                        continue;
                    }

                    //auto-właściwość zapisujemy pod jej nazwą, a nie <Nazwa>k__BackingField
                    var name = field.Name.StartsWith('<') && field.Name.EndsWith(">k__BackingField")
                        ? field.Name.Substring(1, field.Name.IndexOf('>') - 1)
                        : field.Name;

                    //pole prywatne o tej samej nazwie w klasie bazowej
                    var key = result.ContainsKey(name) ? $"{current.FullName}::{name}" : name;
                    result.Add(key, field);
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Wartości zapisywane wprost w atrybutach, bez tożsamości obiektu
    /// </summary>
    internal static class SimpleValues
    {
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            MaxDepth = FieldGraphSerializer.MaxDepth,
        };

        private static readonly (string Name, StringComparer Comparer)[] stringComparers =
        {
            ("Ordinal", StringComparer.Ordinal),
            ("OrdinalIgnoreCase", StringComparer.OrdinalIgnoreCase),
            ("InvariantCulture", StringComparer.InvariantCulture),
            ("InvariantCultureIgnoreCase", StringComparer.InvariantCultureIgnoreCase),
        };

        public static bool TryEncode(object value, out string code, out string text)
        {
            var c = CultureInfo.InvariantCulture;
            (code, text) = value switch
            {
                bool v => ("bool", v ? "1" : "0"),
                byte v => ("u1", v.ToString(c)),
                sbyte v => ("i1", v.ToString(c)),
                short v => ("i2", v.ToString(c)),
                ushort v => ("u2", v.ToString(c)),
                int v => ("i4", v.ToString(c)),
                uint v => ("u4", v.ToString(c)),
                long v => ("i8", v.ToString(c)),
                ulong v => ("u8", v.ToString(c)),
                float v => ("r4", v.ToString("R", c)),
                double v => ("r8", v.ToString("R", c)),
                char v => ("char", ((int)v).ToString(c)),
                decimal v => ("dec", v.ToString(c)),
                DateTime v => ("date", v.ToBinary().ToString(c)),
                DateTimeOffset v => ("dto", v.ToString("O", c)),
                TimeSpan v => ("time", v.Ticks.ToString(c)),
                Guid v => ("guid", v.ToString("D")),
                Uri v => ("uri", v.OriginalString),
                Type v => ("type", TypeNames.Get(v)),
                MethodBase v => ("method", MemberIdentity.Get(v)),
                FieldInfo v => ("field", MemberIdentity.Get(v)),
                RuntimeTypeHandle v => ("typeh", TypeNames.Get(Type.GetTypeFromHandle(v)!)),
                RuntimeFieldHandle v => ("fieldh", MemberIdentity.Get(FieldInfo.GetFieldFromHandle(v))),
                CultureInfo v => ("culture", v.Name),
                //kopia po wartości: węzeł po odczycie nie zna rodzica i nie jest współdzielony
                JsonNode v => ("json", v.ToJsonString(jsonOptions)),
                _ => (null!, null!),
            };

            if (code != null)
            {
                return true;
            }

            if (value is StringComparer comparer)
            {
                foreach (var known in stringComparers)
                {
                    if (known.Comparer.Equals(comparer))
                    {
                        code = "strcmp";
                        text = known.Name;
                        return true;
                    }
                }
            }

            return false;
        }

        public static object Decode(string code, string text)
        {
            var c = CultureInfo.InvariantCulture;
            return code switch
            {
                "bool" => text == "1",
                "u1" => byte.Parse(text, c),
                "i1" => sbyte.Parse(text, c),
                "i2" => short.Parse(text, c),
                "u2" => ushort.Parse(text, c),
                "i4" => int.Parse(text, c),
                "u4" => uint.Parse(text, c),
                "i8" => long.Parse(text, c),
                "u8" => ulong.Parse(text, c),
                "r4" => float.Parse(text, c),
                "r8" => double.Parse(text, c),
                "char" => (char)int.Parse(text, c),
                "dec" => decimal.Parse(text, c),
                "date" => DateTime.FromBinary(long.Parse(text, c)),
                "dto" => DateTimeOffset.ParseExact(text, "O", c),
                "time" => TimeSpan.FromTicks(long.Parse(text, c)),
                "guid" => Guid.ParseExact(text, "D"),
                "uri" => new Uri(text, UriKind.RelativeOrAbsolute),
                "type" => TypeNames.Resolve(text),
                "method" => MemberIdentity.ResolveMethod(text),
                "field" => MemberIdentity.ResolveField(text),
                "typeh" => TypeNames.Resolve(text).TypeHandle,
                "fieldh" => MemberIdentity.ResolveField(text).FieldHandle,
                "culture" => CultureInfo.GetCultureInfo(text),
                "json" => JsonNode.Parse(text, null, new JsonDocumentOptions { MaxDepth = FieldGraphSerializer.MaxDepth })!,
                "strcmp" => stringComparers.First(k => k.Name == text).Comparer,
                _ => throw new StateSerializationException($"Nieznany rodzaj wartości w stanie: {code}"),
            };
        }
    }
}
