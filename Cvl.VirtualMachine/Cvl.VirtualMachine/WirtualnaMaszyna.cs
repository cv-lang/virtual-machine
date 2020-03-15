using Cvl.VirtualMachine.Core;
using Cvl.VirtualMachine.Instructions;
using Cvl.VirtualMachine.Instructions.Calls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine
{
    public class WirtualnaMaszyna
    {
        public HardwareContext HardwareContext { get; set; } = new HardwareContext();
        public bool CzyWykonywacInstrukcje { get; private set; } = true;

        private InstructionsFactory instructionsFactory = new InstructionsFactory();
        public long NumerIteracji { get; set; }
        private InstructionBase aktualnaInstrukcja;

        public Metoda AktualnaMetoda { get; set; }

        internal static void HibernateVirtualMachine()
        {
            throw new NotImplementedException();
        }

        internal static void EndProcessVirtualMachine()
        {
            throw new NotImplementedException();
        }

        public void WykonajMetode()
        {

        }

        public void Start(string nazwaMetody, object process)
        {
            var typ = process.GetType();
            var startMethod = typ.GetMethod(nazwaMetody);//typDef.Methods.FirstOrDefault(mm => mm.Name == nazwaMetodyStartu);
            var m = new Metoda(startMethod);
            HardwareContext.Stos.PushObject(process);

            m.Instrukcje = new List<InstructionBase>() { new CallStart(m) };
        }

        public void Execute()
        {
            //NS.Debug.VM = this; // do debugowania 

            while (CzyWykonywacInstrukcje)
            {
                try
                {
                    //if (NS.Debug.StopIterationNumber == NumerIteracji)
                    //{
                    //    System.Diagnostics.Debugger.Break();
                    //}
                    aktualnaInstrukcja = PobierzAktualnaInstrukcje();
                    aktualnaInstrukcja.Wykonaj();
                    NumerIteracji++;
                }
                catch (Exception ex)
                {
                    CzyWykonywacInstrukcje = false;
                    //Status = VirtualMachineState.Exception;
                    //RzuconyWyjatekCalosc = ex.ToString();
                    //RzuconyWyjatekWiadomosc = ex.Message; ;
                    //if (Debugger.IsAttached)
                    //{
                    //    Debugger.Break();
                    //}
                    return;
                }
            }
        }


        public InstructionBase PobierzAktualnaInstrukcje()
        {
            var ai = AktualnaMetoda.Instrukcje[AktualnaMetoda.NumerWykonywanejInstrukcji];
            ai.WirtualnaMaszyna = this;
            return ai;
        }

        public void WalidujMetodyObiektu(object instancjaObiektu)
        {
            var typ = instancjaObiektu.GetType();
            //var foldre = typ.Assembly.Location;
            //var module = Mono.Cecil.ModuleDefinition.ReadModule(foldre);
            //var typy = module.GetTypes();

            //var typDef = typy.First(t => t.FullName == typ.FullName);
            //var metody = typDef.Methods;
            foreach (var metoda in typ.GetMethods())
            {
                var m = new Metoda(metoda);
                var i = m.PobierzInstrukcjeMetody(); //pobierma instrukcje metody - jeśli brakuje jakiejś instrukcji rzuca wyjątek
            }
        }
    }
}
