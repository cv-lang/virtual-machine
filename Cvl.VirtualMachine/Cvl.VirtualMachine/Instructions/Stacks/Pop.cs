using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Special
{
    public class Pop : InstructionBase
    {        
        public override void Wykonaj()
        {
            HardwareContext.PopObject();
            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}
