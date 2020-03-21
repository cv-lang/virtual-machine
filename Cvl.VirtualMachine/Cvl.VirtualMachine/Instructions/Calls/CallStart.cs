using Cvl.VirtualMachine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Calls
{
    public class CallStart : InstructionBase
    {
        public CallStart(Metoda metoda)
        {
            Metoda = metoda;
        }

        public Metoda Metoda { get; set; }

        public override void Wykonaj()
        {
            HardwareContext.WczytajLokalneArgumenty(1);
            var instancja = HardwareContext.PobierzLokalnyArgument(0);

            Metoda.WyczyscInstrukcje();

            HardwareContext.AktualnaMetoda = Metoda;
            HardwareContext.AktualnaMetoda.NumerWykonywanejInstrukcji = 0;
        }

        
    }
}
