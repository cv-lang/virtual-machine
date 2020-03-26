using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    ///Compares two values. If the first value is greater than the second, 
    /// the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is 
    /// pushed onto the evaluation stack.
    public class Cgt : InstructionBase
    {        
        public override void Wykonaj()
        {
            dynamic b = HardwareContext.PopObject();
            dynamic a = HardwareContext.PopObject();

            dynamic wynik = a > b ? 1 : 0;
            HardwareContext.PushObject(wynik);
            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}