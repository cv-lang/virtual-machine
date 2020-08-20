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
            WczytajLokalneArgumenty(1);
            var instancja = PobierzLokalnyArgument(0);
            
            var metodaDoWykonania = new Metoda();
            metodaDoWykonania.WyczyscInstrukcje();
            metodaDoWykonania.WczytajInstrukcje();

            this.MethodContext.AktualnaMetoda = metodaDoWykonania;
        }

        
    }
}
