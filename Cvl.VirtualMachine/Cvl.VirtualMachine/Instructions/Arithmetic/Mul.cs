using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Arithmetic
{
    public class Mul : InstructionBase
    {
        public override void Wykonaj()
        {
            dynamic b = HardwareContext.PopObject();
            dynamic a = HardwareContext.PopObject();

            dynamic wynik = a * b;
            HardwareContext.PushObject(wynik);
            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}
