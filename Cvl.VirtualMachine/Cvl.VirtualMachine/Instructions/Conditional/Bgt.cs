using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    /// <summary>
    /// Transfers control to a target instruction (short form) if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_un_s
    /// </summary>
    public class Bgt : InstructionBase
    {

        public override void Wykonaj()
        {
            var value2 = PopObject() as dynamic;
            var value1 = PopObject() as dynamic;
            dynamic d = value1 > value2;

            if (d)
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