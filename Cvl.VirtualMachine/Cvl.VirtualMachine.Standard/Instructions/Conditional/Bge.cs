using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    /// <summary>
    /// Transfers control to a target instruction if the first value is greater than or equal to the second value.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge
    /// </summary>
    public class Bge : InstructionBase
    {
        public override void Wykonaj()
        {
            var value2 = PopObject() as dynamic;
            var value1 = PopObject() as dynamic;

            var r = value1 > value2;

            if (r == true)
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
