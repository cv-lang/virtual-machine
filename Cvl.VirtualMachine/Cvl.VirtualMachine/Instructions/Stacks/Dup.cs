using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Stacks
{
    /// <summary>
    /// Copies the current topmost value on the evaluation stack, and then pushes the copy onto the evaluation stack.
    /// </summary>
    public class Dup : InstructionBase
    {
        public override void Wykonaj()
        {
            var o = PopObject();
            PushObject(o);
            PushObject(o);

            WykonajNastepnaInstrukcje();
        }
    }
}
