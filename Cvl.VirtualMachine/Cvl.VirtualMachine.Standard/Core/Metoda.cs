using Cvl.VirtualMachine.Core.Tools;
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
    /// </summary>
    public class Metoda : ElementBase
    {
        public Metoda()
        {
            LocalArguments = new MethodData();
            LocalVariables = new MethodData();
            instrukcje = null;
        }

        public Metoda(MethodInfo metoda, VirtualMachine wirtualnaMaszyna) : this()
        {
            methodInfo = metoda;
            var m = this;
            m.AssemblyName = metoda.Module.FullyQualifiedName;
            m.NazwaTypu = metoda.DeclaringType.FullName;
            m.NazwaMetody = metoda.Name;
            m.NumerWykonywanejInstrukcji = 0;
            Xml = Serializer.SerializeObject(metoda.DeclaringType);
            WirtualnaMaszyna = wirtualnaMaszyna;
        }

        #region Propercje

        private MethodInfo methodInfo;

        public string NazwaTypu { get; set; }
        public string NazwaMetody { get; set; }
        public string AssemblyName { get; internal set; }
        public int OffsetWykonywanejInstrukcji { get; internal set; }
        public string Xml { get; set; }

        public VirtualMachine WirtualnaMaszyna { get; set; }


        public int NumerWykonywanejInstrukcji { get; set; }
        public MethodData LocalArguments { get; set; }
        public MethodData LocalVariables { get; set; }

        private List<InstructionBase> instrukcje;
        internal List<InstructionBase> Instrukcje
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

        public List<InstructionBase> PobierzInstrukcjeMetody()
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

                }
            }

            return list;
        }

        #endregion









        public int PobierzNumerInstrukcjiZOffsetem(int offset)
        {
            var inst = Instrukcje.FirstOrDefault(i => i.Instruction.Offset == offset);
            return Instrukcje.IndexOf(inst);
        }

        public MethodInfo PobierzOpisMetody()
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



        public bool CzyObslugujeWyjatki()
        {
            var metoda = PobierzOpisMetody();
            return metoda.GetMethodBody().ExceptionHandlingClauses.Count > 0;
        }

        public override string ToString()
        {
            return NazwaMetody + " 0x" + OffsetWykonywanejInstrukcji.ToString("X") + " " + NazwaTypu + " " + AssemblyName;
        }
    }
}