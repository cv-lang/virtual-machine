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
                case "beq":
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
                case "brzero":
                case "brzero.s":
                    return CreateInstruction<Brzero>(instrukcja);
                case "brfalse":
                case "brfalse.s":
                    return CreateInstruction<Brfalse>(instrukcja);
                case "brnull":
                    return CreateInstruction<Brnull>(instrukcja);
                case "brtrue":
                case "brtrue.s":
                    return CreateInstruction<Brtrue>(instrukcja);
                case "brinst":
                    return CreateInstruction<Brinst>(instrukcja);
                case "isinst":
                    return CreateInstruction<Isinst>(instrukcja);
                case "bne.un.s":
                    return CreateInstruction<Bne>(instrukcja);
                case "bgt.s":
                case "bgt.un.s":
                case "bgt.un":
                    return CreateInstruction<Bgt>(instrukcja);
                case "blt":
                case "blt.s":
                    return CreateInstruction<Blt>(instrukcja);
                case "bge":
                case "bge.s":
                    return CreateInstruction<Bge>(instrukcja);
                case "ble":
                case "ble.s":
                    return CreateInstruction<Ble>(instrukcja);                
            }
            return null;
        }
    }
}
