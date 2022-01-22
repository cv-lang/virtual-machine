using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    /// <summary>
    /// Unconditionally transfers control to a target instruction.
    /// </summary>
    public class Br : InstructionBase
    {
        public override void Wykonaj()
        {
            var op = Instruction.Operand as Instruction;
            var nextOffset = op.Offset;
            WykonajSkok(nextOffset);
        }
    }
}
