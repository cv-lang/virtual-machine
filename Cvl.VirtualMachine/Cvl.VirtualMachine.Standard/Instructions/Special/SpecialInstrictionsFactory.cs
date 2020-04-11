using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Special
{
    public class SpecialInstrictionsFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "nop":
                    return CreateInstruction<Nop>(instrukcja);
            }
            return null;
        }
    }
}
