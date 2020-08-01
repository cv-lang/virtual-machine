using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    /// <summary>
    /// Transfers control to a target instruction (short form) if two values are equal.
    /// https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.beq_s(v=vs.110).aspx
    /// </summary>
    public class Beq : InstructionBase
    {       

        public override void Wykonaj()
        {
            var value2 = HardwareContext.PopObject();
            var value1 = HardwareContext.PopObject();

            if ((value1==null && value2== null) || value2.Equals(value1))
            {
                var op = Instruction.Operand as Instruction;
                var nextOffset = op.Offset;
                HardwareContext.WykonajSkok(nextOffset);
            } else
            {
                HardwareContext.WykonajNastepnaInstrukcje();
            }
        }
    }
}
