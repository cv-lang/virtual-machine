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
                case "shl.un":
                    return CreateInstruction<Shl>(instrukcja);
                case "shr":
                case "shr.un":
                    return CreateInstruction<Shr>(instrukcja);
            }
            return null;
        }
    }
}
