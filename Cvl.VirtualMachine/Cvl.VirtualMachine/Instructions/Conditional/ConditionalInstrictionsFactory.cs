using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    public class ConditionalInstrictionsFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "br.s":
                case "br":
                    return CreateInstruction<Br>(instrukcja);
            }
            return null;
        }
    }
}
