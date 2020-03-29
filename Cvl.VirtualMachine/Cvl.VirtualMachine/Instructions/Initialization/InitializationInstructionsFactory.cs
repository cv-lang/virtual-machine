using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Initialization
{
    public class InitializationInstructionsFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "initobj":
                    return CreateInstruction<Initobj>(instrukcja);
                case "newobj":
                    return CreateInstruction<Newobj>(instrukcja);
                case "newarr":
                    return CreateInstruction<Newarr>(instrukcja);
            }
            return null;
        }
    }
}
