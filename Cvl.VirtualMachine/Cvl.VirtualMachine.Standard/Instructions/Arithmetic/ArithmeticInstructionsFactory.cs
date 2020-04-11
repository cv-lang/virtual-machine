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
                case "sub":
                    return CreateInstruction<Sub>(instrukcja);
                case "mul":
                    return CreateInstruction<Mul>(instrukcja);
                case "div":
                    return CreateInstruction<Div>(instrukcja);
            }
            return null;
        }
    }
}
