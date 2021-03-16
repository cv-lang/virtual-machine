using System;
using System.Collections.Generic;
using System.Text;
using Mono.Reflection;

namespace Cvl.VirtualMachine.Instructions.Shift
{
    public class ShiftInstructionsFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "shl":
                    return CreateInstruction<Shl>(instrukcja);
                case "shr":
                    return CreateInstruction<Shr>(instrukcja);
            }
            return null;
        }
    }
}
