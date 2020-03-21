using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Arithmetic
{
    public class ArithmeticInstructionsFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "add":
                    return CreateInstruction<Add>(instrukcja);
            }
            return null;
        }
    }
}
