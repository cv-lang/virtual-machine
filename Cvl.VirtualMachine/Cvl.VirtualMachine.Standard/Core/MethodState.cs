using Cvl.ApplicationServer.Logs;
using Cvl.VirtualMachine.Core.Tools;
using Cvl.VirtualMachine.Core.Variables;
using Cvl.VirtualMachine.Core.Variables.Addresses;
using Cvl.VirtualMachine.Instructions;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cvl.VirtualMachine.Core
{
    /// <summary>
    /// Metoda która będzie wykonywana
    /// https://www.ecma-international.org/wp-content/uploads/ECMA-335_6th_edition_june_2012.pdf p.108
    /// </summary>
    public class MethodState : ElementBase
    {
        public MethodState()
        {
            LocalArguments = new MethodData();
            LocalVariables = new MethodData();
            instrukcje = null;
        }

        public MethodState(MethodBase metoda, VirtualMachine wirtualnaMaszyna, object instance) : this()
        {          
            if(instance != null & metoda.IsVirtual)
            {
                //mamy wirtualną metodę, pobieramy z typu
                methodInfo = instance.GetType().GetMethod(metoda.Name, 
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }
            else
            {
                methodInfo = metoda;
            }

            CzyStatyczna = methodInfo.IsStatic;
            
            var m = this;
            m.AssemblyName = methodInfo.Module.FullyQualifiedName;
            m.NazwaTypu = methodInfo.DeclaringType.FullName;
            m.NazwaMetody = methodInfo.Name;
            m.NumerWykonywanejInstrukcji = 0;
            Xml = Serializer.SerializeObject(methodInfo.DeclaringType);
            WirtualnaMaszyna = wirtualnaMaszyna;
            
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
                
        public Type ConstrainedType { get; internal set; }

        public bool CzyWykonywacInstrukcje { get; set; } = true;


        /// <summary>
        /// Czy dana metoda jest statyczna czy zawiera this
        /// </summary>
        public bool CzyStatyczna { get; set; } = false;


        private MethodBase methodInfo;

        public string NazwaTypu { get; set; }
        public string NazwaMetody { get; set; }
        public string AssemblyName { get; internal set; }
       
        public string Xml { get; set; }

        public VirtualMachine WirtualnaMaszyna { get; set; }



        /// <summary>
        /// Numer wykonywanej instrukcji (0 - to pierwsza instrukcja, 1 - to druga itd.)
        /// </summary>
        public int NumerWykonywanejInstrukcji { get; set; }

        /// <summary>
        /// Offset wykonywanej instrukcji w bajtach - 
        /// instrukcje mogą mieć rozmiar 1, 2, 4 bajtowy lub więcej
        /// Offset zawiera pozycję instukcji w binarnym bytecode programu
        /// </summary>
        public int OffsetWykonywanejInstrukcji { get; internal set; }
                

        private List<InstructionBase> instrukcje;
        public List<InstructionBase> Instrukcje
        {
            get
            {                
                return instrukcje;
            }
            set { instrukcje = value; }
        }

        public Logger Logger { get; internal set; }

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
            am.OffsetWykonywanejInstrukcji
                = am.Instrukcje[am.NumerWykonywanejInstrukcji].Instruction.Offset;
        }

        public void WykonajSkok(int nowyOffset)
        {
            var am = this;
            var ins = am.Instrukcje.FirstOrDefault(i => i.Instruction.Offset == nowyOffset);
            am.NumerWykonywanejInstrukcji = am.Instrukcje.IndexOf(ins);
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
            if (methodInfo != null)
            {
                return methodInfo;
            }
            else
            {
                var obiektTyp = Serializer.DeserializeObject(Xml);
                var typ = (Type)obiektTyp;
                return typ.GetMethod(NazwaMetody);
                //var module = Assembly.LoadFile(this.AssemblyName);
                //var typDef = module.GetTypes().First(t => t.FullName == NazwaTypu);
                //var metoda = typDef.GetMethods().FirstOrDefault(mm => mm.Name == NazwaMetody);
                //return metoda;
            }
        }

       /// <summary>
       /// Zwraca wszystkie bloki w których jest obecny punkt wykonywania 
       /// </summary>
       /// <returns></returns>
        public List<ExceptionHandlingClause> PobierzBlokiObslugiWyjatkow()
        {
            var lista = new List<ExceptionHandlingClause>();
            var exceptionClauses = PobierzOpisMetody().GetMethodBody().ExceptionHandlingClauses;
            int offset = OffsetWykonywanejInstrukcji;

            foreach (var item in exceptionClauses)
            {
                if (item.TryOffset < offset && (item.TryOffset + item.TryLength) > offset)
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
            var exceptionClauses = PobierzOpisMetody().GetMethodBody().ExceptionHandlingClauses;
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
            return $"{NazwaMetody} {Instrukcje[this.NumerWykonywanejInstrukcji]}";
        }
    }
}