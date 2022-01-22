using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Initialization
{
    /// <summary>
    /// Pushes an object reference to a new zero-based, one-dimensional array whose elements are of a specific type onto the evaluation stack.
    /// </summary>
    public class Newarr : InstructionBase
    {
        public override void Wykonaj()
        {
            //throw new NotImplementedException("instrukcja Newarr");
            var tr = Instruction.Operand as Type;
            //var td = tr;

            var typ = tr;
            var n = (int)PopObject();
            object arr = Array.CreateInstance(typ, n);
            PushObject(arr);
            WykonajNastepnaInstrukcje();
        }

    }
}
