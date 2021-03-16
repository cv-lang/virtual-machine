using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Logic
{
    public class LogicInstructionFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "and":
                    return CreateInstruction<And>(instrukcja);
                case "neg":
                    return CreateInstruction<Neg>(instrukcja);
                case "or":
                    return CreateInstruction<Or>(instrukcja);
                case "xor":
                    return CreateInstruction<Xor>(instrukcja);
            }
            return null;
        }
    }
}
