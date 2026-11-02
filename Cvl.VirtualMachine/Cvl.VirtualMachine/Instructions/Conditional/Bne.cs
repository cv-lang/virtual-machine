using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    /// <summary>
    /// Transfers control to a target instruction (short form) when two unsigned integer values or unordered float values are not equal.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bne_un_s
    /// </summary>
    public class Bne : InstructionBase
    {
        public override void Wykonaj()
        {
            var value2 = PopObject();
            var value1 = PopObject();

            if (!CzyRowne(value1, value2))
            {
                var op = Instruction.Operand as Instruction;
                var nextOffset = op.Offset;
                WykonajSkok(nextOffset);
            }
            else
            {
                WykonajNastepnaInstrukcje();
            }
        }
    }
}
