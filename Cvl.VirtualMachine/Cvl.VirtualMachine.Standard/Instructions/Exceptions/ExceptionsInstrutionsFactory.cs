using Cvl.VirtualMachine.Instructions.Special;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Exceptions
{
    public class ExceptionsInstrutionsFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "throw":
                    return CreateInstruction<Throw>(instrukcja);
                case "endfinally":
                    return CreateInstruction<Endfinally>(instrukcja);
                case "leave.s":
                    return CreateInstruction<Leave_S>(instrukcja);
            }
            return null;
        }
    }
}
