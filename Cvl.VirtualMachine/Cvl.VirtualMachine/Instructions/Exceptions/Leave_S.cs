using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Exceptions
{
    public class Leave_S : InstructionBase
    {
        public override void Wykonaj()
        {
            var i = Instruction.Operand as Instruction;
            var nextOffset = i.Offset;
            HardwareContext.WykonajSkok(nextOffset);
        }
    }
}
