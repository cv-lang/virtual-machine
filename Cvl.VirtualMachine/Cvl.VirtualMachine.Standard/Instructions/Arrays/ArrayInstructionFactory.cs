using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Arrays
{
    public class ArrayInstructionFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "stelem.ref":
                    return CreateInstruction<Stelem>(instrukcja);                
            }
            return null;
        }
    }
}
