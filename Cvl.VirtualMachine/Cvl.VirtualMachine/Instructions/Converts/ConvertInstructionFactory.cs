using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Converts
{
    public class ConvertInstructionFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "conv.i4":
                    return CreateInstruction<Conv>(instrukcja, i=> i.ConvertType = ConvertType.i4);
                
            }
            return null;
        }
    }
}
