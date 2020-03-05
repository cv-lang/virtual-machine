using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Special
{
    public class Nop : InstructionBase
    {
        public override void Wykonaj()
        {
            WykonajNastepnaInstrukcje();
        }
    }
}
