using Cvl.VirtualMachine.Core.Tools;
using Cvl.VirtualMachine.Core.Variables;
using Cvl.VirtualMachine.Core.Variables.Addresses;
using Cvl.VirtualMachine.Instructions;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using Cvl.VirtualMachine.Core.Serializers;

namespace Cvl.VirtualMachine.Core
{
    /// <summary>
    /// Metoda która będzie wykonywana
    /// https://www.ecma-international.org/wp-content/uploads/ECMA-335_6th_edition_june_2012.pdf p.108
    /// </summary>
    public class MethodState : ElementBase , IDeserializedObject
    {
        

        public MethodState(MethodBase metoda, VirtualMachine wirtualnaMaszyna, object instance) : this()
        {          
            if(instance != null & metoda.IsVirtual)
            {
                //mamy wirtualną metodę, pobieramy z typu
                methodInfo = PobierzImplementacje((MethodInfo)metoda, instance.GetType());
            }
            else
            {
                methodInfo = metoda;
            }

            if (methodInfo.IsDefined(typeof(AsyncStateMachineAttribute), false))
            {
                throw new NotSupportedException(
                    $"Metoda async {methodInfo.DeclaringType.FullName}.{methodInfo.Name} nie może być interpretowana - "
                    + "jej stanu nie da się zapisać. Usuń [Interpret] i poczekaj na wynik przed hibernacją.");
            }

            CzyStatyczna = methodInfo.IsStatic;
            
            var m = this;
            m.NazwaTypu = methodInfo.DeclaringType.FullName;
            m.NazwaMetody = methodInfo.Name;
            m.MethodFullName = $"{methodInfo.DeclaringType.FullName}.{methodInfo.Name}";
            //m.NumerWykonywanejInstrukcji = 0;
            MethodIdentity = Serializers.MemberIdentity.Get(methodInfo);
            WirtualnaMaszyna = wirtualnaMaszyna;


            if (methodInfo.GetMethodBody() != null && methodInfo.GetMethodBody().InitLocals == true)
            {
                int i = 0;
                foreach (var localVariableInfo in methodInfo.GetMethodBody().LocalVariables)
                {
                    var inst = DomyslnaWartosc(localVariableInfo.LocalType);
                    LocalVariables.Ustaw(i, inst);
                    i++;
                }
            }

        }

        private static MethodBase PobierzImplementacje(MethodInfo metoda, Type typ)
        {
            if (metoda.DeclaringType.IsInterface)
            {
                var mapa = typ.GetInterfaceMap(metoda.DeclaringType);
                var indeks = Array.IndexOf(mapa.InterfaceMethods, metoda.IsGenericMethod ? metoda.GetGenericMethodDefinition() : metoda);
                if (indeks < 0)
                {
                    return metoda;
                }
                var implementacja = mapa.TargetMethods[indeks];
                return metoda.IsGenericMethod ? implementacja.MakeGenericMethod(metoda.GetGenericArguments()) : implementacja;
            }

            var definicja = metoda.IsGenericMethod ? metoda.GetGenericMethodDefinition() : metoda;
            var bazowa = definicja.GetBaseDefinition();
            for (var t = typ; t != null; t = t.BaseType)
            {
                foreach (var m in t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (m.Name == metoda.Name && m.GetBaseDefinition() == bazowa)
                    {
                        return metoda.IsGenericMethod ? m.MakeGenericMethod(metoda.GetGenericArguments()) : m;
                    }
                }
            }
            return metoda;
        }

        /// <summary>
        /// Wartość zmiennej lokalnej przed pierwszym przypisaniem (InitLocals) - jak w CLR
        /// </summary>
        private static object DomyslnaWartosc(Type type)
        {
            if (type == typeof(DefaultInterpolatedStringHandler))
            {
                //tworzy go dopiero konstruktor wywołany przez Call
                return null;
            }

            if (type.IsByRefLike || type.IsDefined(typeof(InlineArrayAttribute), false))
            {
                throw new NotSupportedException(
                    $"ref struct are not supported (IsByRefLike type): {type}. Metoda interpretowana nie może używać Span ani params ReadOnlySpan - "
                    + "przekaż tablicę, np. string.Join(\"|\", new[] { a, b }), albo przenieś ten kod do metody bez [Interpret].");
            }

            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public MethodState()
        {
            LocalArguments = new MethodData();
            LocalVariables = new MethodData();
            instrukcje = null;
        }

        public void AfterDeserialization()
        {
            WczytajInstrukcje();
        }

        #region Propercje

        /// <summary>
        /// Zmienne z którymi została wywołana metoda
        /// </summary>
        public MethodData LocalArguments { get; set; }

        /// <summary>
        /// Lokalne zmienne metody
        /// </summary>
        public MethodData LocalVariables { get; set; }

        /// <summary>
        /// Stos wykonywania metody - tu są zmienne tymczasowe metody 
        /// (taki jej prywatny stos)
        /// </summary>
        public EvaluationStack EvaluationStack { get; set; } = new EvaluationStack();

        /// <summary>
        /// Stos bloków try i cachy
        /// </summary>
        public TryCatchStack TryCatchStack { get; set; } = new TryCatchStack();

        public Type ConstrainedType { get; set; }

        public bool CzyWykonywacInstrukcje { get; set; } = true;


        /// <summary>
        /// Czy dana metoda jest statyczna czy zawiera this
        /// </summary>
        public bool CzyStatyczna { get; set; } = false;


        [NonSerialized]
        private MethodBase methodInfo;

        [NonSerialized]
        private IList<ExceptionHandlingClause> klauzule;

        public string NazwaTypu { get; set; }
        public string NazwaMetody { get; set; }
        public string MethodFullName { get; set; }

        /// <summary>
        /// Pełna sygnatura metody - po odczycie stanu odtwarza ją jednoznacznie (także prywatną i przeciążoną)
        /// </summary>
        public string MethodIdentity { get; set; }

        public VirtualMachine WirtualnaMaszyna { get; set; }



        /// <summary>
        /// Numer wykonywanej instrukcji (0 - to pierwsza instrukcja, 1 - to druga itd.)
        /// </summary>
        public ExecutionPoints NumerWykonywanejInstrukcji { get; set; } = new ExecutionPoints();

        /// <summary>
        /// Offset wykonywanej instrukcji w bajtach - 
        /// instrukcje mogą mieć rozmiar 1, 2, 4 bajtowy lub więcej
        /// Offset zawiera pozycję instukcji w binarnym bytecode programu
        /// </summary>
        public int OffsetWykonywanejInstrukcji => Instrukcje[NumerWykonywanejInstrukcji.CurrentInstructionIndex].Instruction.Offset;
                

        [NonSerialized]
        private List<InstructionBase> instrukcje;

        [XmlIgnore]
        public List<InstructionBase> Instrukcje
        {
            get
            {
                return instrukcje;
            }
            set { instrukcje = value; }
        }

        #endregion


        #region Instrukcje

        public void WyczyscInstrukcje()
        {
            instrukcje = null;
        }

        public void WczytajInstrukcje()
        {
            instrukcje = PobierzInstrukcjeMetody();
        }

        internal List<InstructionBase> PobierzInstrukcjeMetody()
        {
            var metoda = PobierzOpisMetody();
            //metoda
            var il = metoda.GetInstructions();
            var list = new List<InstructionBase>();
            var factory = WirtualnaMaszyna.instructionsFactory;

            foreach (var item in il)
            {
                var i = factory.UtworzInstrukcje(item, WirtualnaMaszyna);
                if (i != null)
                {
                    list.Add(i);
                }
                else
                {
                    //niema instrukcji
                    throw new Exception($"Brak instrukcji {item}");
                }
            }

            return list;
        }

        #endregion

        #region Execution flow

        public void WykonajNastepnaInstrukcje()
        {
            var am = this;
            am.NumerWykonywanejInstrukcji++;
        }

        public void WykonajSkok(int nowyOffset)
        {
            var am = this;
            var instructionIndex = PobierzNumerInstrukcjiZOffsetem(nowyOffset);
            am.NumerWykonywanejInstrukcji.SetCurrentInstructionIndex(instructionIndex);
        }

        #endregion

        #region Stack

        public void PushObject(object o)
        {
            EvaluationStack.PushObject(o);
        }

        public void Push(ElementBase o)
        {
            EvaluationStack.Push(o);
        }

        /// <summary>
        /// Zwraca obiekt
        /// jeśli jest adres na stosie to zamienia na obiekt
        /// </summary>
        /// <returns></returns>
        public object PopObject()
        {
            var ob = EvaluationStack.Pop();
            if (ob is ObjectWraperBase)
            {
                var v = ob as ObjectWraperBase;
                return v.GetValue();
            }

            return ob;
        }

        public object Pop()
        {
            var ob = EvaluationStack.Pop();

            return ob;
        }



        #endregion

        #region Local args

        public void WczytajLokalneArgumenty(int iloscArgumentow)
        {
            var lista = new object[iloscArgumentow];
            for (int i = iloscArgumentow - 1; i >= 0; i--)
            {
                var o = EvaluationStack.Pop();
                lista[i] = o;
            }
            LocalArguments.Wczytaj(lista);
        }

        public void ZapiszLokalnyArgument(object o, int indeks)
        {
            var index = indeks;
            if(CzyStatyczna == false)
            {
                //index++;
            }

            LocalArguments.Ustaw(index, o);
        }

        public object PobierzLokalnyArgument(int indeks)
        {
            var index = indeks;
            if (CzyStatyczna == false)
            {
                //index++;
            }

            var obiekt = LocalArguments.Pobierz(index);
            var ow = obiekt as ObjectWraperBase;
            if (ow != null)
            {
                return ow.GetValue();
            }
            return obiekt;
        }

        public ArgumentAddress PobierzAdresArgumentu(int indeks)
        {
            var index = indeks;
            if (CzyStatyczna == false)
            {
                //index++;
            }

            var adres = new ArgumentAddress();
            adres.Indeks = index;
            adres.LokalneArgumenty = LocalArguments;

            return adres;
        }

        #endregion

        #region Local var

        public void ZapiszLokalnaZmienna(object o, int indeks)
        {
            LocalVariables.Ustaw(indeks, o);
        }

        public object PobierzLokalnaZmienna(int indeks)
        {
            var obiekt = LocalVariables.Pobierz(indeks);
            var ow = obiekt as ObjectWraperBase;
            if (ow != null)
            {
                return ow.GetValue();
            }
            return obiekt;
        }

        public LocalVariableAddress PobierzAdresZmiennejLokalnej(int indeks)
        {
            var adres = new LocalVariableAddress();
            adres.Indeks = indeks;
            adres.LokalneZmienne = LocalVariables;

            return adres;
        }

        #endregion

       

        public int PobierzNumerInstrukcjiZOffsetem(int offset)
        {
            var inst = Instrukcje.FirstOrDefault(i => i.Instruction.Offset == offset);
            return Instrukcje.IndexOf(inst);
        }

        public MethodBase PobierzOpisMetody()
        {
            return methodInfo ??= Serializers.MemberIdentity.ResolveMethod(MethodIdentity);
        }

        internal IList<ExceptionHandlingClause> Klauzule => klauzule ??= PobierzOpisMetody().GetMethodBody().ExceptionHandlingClauses;

       /// <summary>
       /// Zwraca wszystkie bloki w których jest obecny punkt wykonywania 
       /// </summary>
       /// <returns></returns>
        public List<ExceptionHandlingClause> PobierzBlokiObslugiWyjatkow()
        {
            var lista = new List<ExceptionHandlingClause>();
            var exceptionClauses = Klauzule;
            int offset = OffsetWykonywanejInstrukcji;

            foreach (var item in exceptionClauses)
            {
                if (item.TryOffset <= offset && (item.TryOffset + item.TryLength) > offset)
                {
                    lista.Add(item);
                }
            }

            return lista;
        }

        /// <summary>
        /// Zwraca rozpoczynające bloki
        /// </summary>
        /// <returns></returns>
        public List<ExceptionHandlingClause> GetBeginTryBlocks()
        {
            var lista = new List<ExceptionHandlingClause>();
            var exceptionClauses = Klauzule;
            int offset = OffsetWykonywanejInstrukcji;

            foreach (var item in exceptionClauses)
            {
                if (item.TryOffset == offset)
                {
                    lista.Add(item);
                }
            }

            return lista;
        }


        public bool CzyObslugujeWyjatki()
        {
            var metoda = PobierzOpisMetody();
            return metoda.GetMethodBody().ExceptionHandlingClauses.Count > 0;
        }

        public override string ToString()
        {
            
            return $"{NazwaTypu}.{NazwaMetody} {Instrukcje[this.NumerWykonywanejInstrukcji.CurrentInstructionIndex]}";
        }

       
    }


    
}