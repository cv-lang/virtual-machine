using Cvl.VirtualMachine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Calls
{
    public class CallStart : InstructionBase
    {
        public CallStart(MethodState metoda)
        {
            Metoda = metoda;
        }

        public MethodState Metoda { get; set; }

        public override void Wykonaj()
        {
            WczytajLokalneArgumenty(1);
            var instancja = PobierzLokalnyArgument(0);
            
            var metodaDoWykonania = new MethodState();
            metodaDoWykonania.WyczyscInstrukcje();
            metodaDoWykonania.WczytajInstrukcje();

            this.MethodContext = metodaDoWykonania;
            this.HardwareContext.AktualnaMetoda = metodaDoWykonania; //TODO: do usuniecia
        }

        
    }
}
