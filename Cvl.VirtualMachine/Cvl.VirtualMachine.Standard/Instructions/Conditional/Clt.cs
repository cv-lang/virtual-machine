using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    /// <summary>
    /// Compares two values. If the first value is less than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
    /// </summary>
    public class Clt : InstructionBase
    {        
        public override void Wykonaj()
        {
            dynamic b = HardwareContext.PopObject();
            dynamic a = HardwareContext.PopObject();

            int one = 1;
            int zero = 0;

            int wynik = a < b ? one : zero;
            HardwareContext.PushObject(wynik);
            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}