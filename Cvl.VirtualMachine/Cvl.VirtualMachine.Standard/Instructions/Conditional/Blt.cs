using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    /// <summary>
    /// Transfers control to a target instruction (short form) if the first value is less than the second value.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_s
    /// </summary>
    public class Blt : InstructionBase
    {
        public override void Wykonaj()
        {
            var value2 = HardwareContext.PopObject() as dynamic;
            var value1 = HardwareContext.PopObject() as dynamic;

            var r = value1 < value2;

            if (r == true)
            {
                var op = Instruction.Operand as Instruction;
                var nextOffset = op.Offset;
                HardwareContext.WykonajSkok(nextOffset);
            }
            else
            {
                HardwareContext.WykonajNastepnaInstrukcje();
            }
        }
    }
}
