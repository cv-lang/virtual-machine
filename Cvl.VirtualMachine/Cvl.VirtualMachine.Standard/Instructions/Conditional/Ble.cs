using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    /// <summary>
    /// Transfers control to a target instruction if the first value is less than or equal to the second value.
    /// 
    /// </summary>
    public class Ble : InstructionBase
    {
        public override void Wykonaj()
        {
            var value2 = HardwareContext.PopObject() as dynamic;
            var value1 = HardwareContext.PopObject() as dynamic;

            var r = value1 <= value2;

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
