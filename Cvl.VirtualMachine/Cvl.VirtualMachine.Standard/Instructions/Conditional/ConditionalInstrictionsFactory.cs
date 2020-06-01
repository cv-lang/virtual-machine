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
                case "beq.s":
                    return CreateInstruction<Beq>(instrukcja);
                case "br.s":
                case "br":
                    return CreateInstruction<Br>(instrukcja);
                case "cgt":
                    return CreateInstruction<Cgt>(instrukcja);
                case "cgt.un":
                    return CreateInstruction<Cgt_Un>(instrukcja);
                case "clt":
                    return CreateInstruction<Clt>(instrukcja);
                case "ceq":
                    return CreateInstruction<Ceq>(instrukcja);
                case "brfalse":
                    return CreateInstruction<Brfalse>(instrukcja);
                case "brfalse.s":
                    return CreateInstruction<Brfalse>(instrukcja);
                case "brtrue":
                case "brtrue.s":
                    return CreateInstruction<Brtrue>(instrukcja);
                case "isinst":
                    return CreateInstruction<Isinst>(instrukcja);
            }
            return null;
        }
    }
}
