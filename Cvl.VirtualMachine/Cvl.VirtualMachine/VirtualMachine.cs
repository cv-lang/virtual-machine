using Cvl.VirtualMachine.Core;
using Cvl.VirtualMachine.Instructions;
using Cvl.VirtualMachine.Instructions.Calls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cvl.VirtualMachine
{
    public class VirtualMachine
    {
        public VirtualMachine()
        {
            HardwareContext = new HardwareContext() { WirtualnaMaszyna = this };
        }

        public HardwareContext HardwareContext { get; set; } 
        public bool CzyWykonywacInstrukcje { get; private set; } = true;

        public InstructionsFactory instructionsFactory = new InstructionsFactory();
        

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

        public void Start(string nazwaMetody, params object[] parametety)
        {
            var process = parametety.First();
            var typ = process.GetType();
            var startMethod = typ.GetMethod(nazwaMetody);//typDef.Methods.FirstOrDefault(mm => mm.Name == nazwaMetodyStartu);
            var m = new Metoda(startMethod, this);
            m.WczytajInstrukcje();
            HardwareContext.AktualnaMetoda = m;
            //HardwareContext.Stos.PushObject(process);
            m.LocalArguments.Wczytaj(parametety);
            

            //m.Instrukcje = new List<InstructionBase>() { new CallStart(m) { HardwareContext = this.HardwareContext } };
            HardwareContext.Execute();
        }

        public T Start<T>(string nazwaMetody, params object[] parametet)
        {
            HardwareContext = new HardwareContext() { WirtualnaMaszyna = this };
            Start(nazwaMetody, parametet);
            var ret = HardwareContext.PopObject();
            return (T)ret;
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
                var m = new Metoda(metoda, this);
                var i = m.PobierzInstrukcjeMetody(); //pobierma instrukcje metody - jeśli brakuje jakiejś instrukcji rzuca wyjątek
            }
        }
    }
}
