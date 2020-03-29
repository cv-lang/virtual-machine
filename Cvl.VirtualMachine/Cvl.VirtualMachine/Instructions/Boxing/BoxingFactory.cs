using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Boxing
{
    public class BoxingFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "box":
                    return CreateInstruction<Box>(instrukcja);
                case "unbox":
                    return CreateInstruction<Unbox>(instrukcja);
                case "unbox.any":
                    return CreateInstruction<Unbox_Any>(instrukcja);
            }
            return null;
        }
    }
}
