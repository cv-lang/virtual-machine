using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Arrays
{
    public class Stelem : InstructionBase
    {
        public override void Wykonaj()
        {
            var val = PopObject();
            var index = (int)PopObject();
            var array = (Array)PopObject();

            array.SetValue(val, index);

            WykonajNastepnaInstrukcje();
        }
    }
}
