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
            var b = PopObject();
            var a = PopObject();
            PushObject(CzyRowne(a, b));
            WykonajNastepnaInstrukcje();
        }
    }
}
