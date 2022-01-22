using Cvl.VirtualMachine.Instructions.Special;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Stacks
{
    public class StacksInstructionsFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "dup":
                    return CreateInstruction<Dup>(instrukcja);
                case "pop":
                    return CreateInstruction<Pop>(instrukcja);
            }
            return null;
        }
    }
}
