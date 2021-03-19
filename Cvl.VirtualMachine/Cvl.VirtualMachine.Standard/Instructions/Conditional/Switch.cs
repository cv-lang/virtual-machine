using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    public class Switch : InstructionBase
    {
        public override void Wykonaj()
        {
            var index = (int)PopObject();
            
            if(index <0)
            {
                WykonajNastepnaInstrukcje();
            } else
            {
                var instructions = (Mono.Reflection.Instruction[])Instruction.Operand;
                var selectedInstruction = instructions[index];
                WykonajSkok(selectedInstruction.Offset);
            }
        }
    }
}
