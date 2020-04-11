using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    //Compares two values. If they are equal, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
    public class Ceq : InstructionBase
    {       

        public override void Wykonaj()
        {
            dynamic b = HardwareContext.PopObject();
            dynamic a = HardwareContext.PopObject();
            if (a is int && b is bool)
            {
                b = b ? 1 : 0;
            }
            else if (b is int && a is bool)
            {
                a = a ? 1 : 0;
            } else if( a is Enum && b is int)
            {
                a = (int)a;
            }

            dynamic wynik = a == b;
            HardwareContext.PushObject(wynik);
            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}