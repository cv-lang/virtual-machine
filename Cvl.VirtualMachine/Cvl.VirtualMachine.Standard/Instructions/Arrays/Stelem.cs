using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Arrays
{
    public class Stelem : InstructionBase
    {
        public override void Wykonaj()
        {
            var val = HardwareContext.PopObject();
            var index = (int)HardwareContext.PopObject();
            var array = (Array)HardwareContext.PopObject();

            array.SetValue(val, index);

            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}
