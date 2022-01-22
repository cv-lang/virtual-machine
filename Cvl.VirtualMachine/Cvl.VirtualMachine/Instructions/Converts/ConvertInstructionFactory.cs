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
                case "conv.ovf.i": //TODO:: zrobić obsługę dla ovf i un
                case "conv.ovf.i.un":
                case "conv.i":
                    return CreateInstruction<Conv>(instrukcja, i => i.ConvertType = ConvertType.i);
                case "conv.ovf.i1":
                case "conv.ovf.i1.un":
                case "conv.i1":
                    return CreateInstruction<Conv>(instrukcja, i => i.ConvertType = ConvertType.i1);
                case "conv.ovf.i2":
                case "conv.ovf.i2.un":
                case "conv.i2":
                    return CreateInstruction<Conv>(instrukcja, i => i.ConvertType = ConvertType.i2);
                case "conv.ovf.i4":
                case "conv.ovf.i4.un":
                case "conv.i4":
                    return CreateInstruction<Conv>(instrukcja, i=> i.ConvertType = ConvertType.i4);
                case "conv.ovf.i8":
                case "	conv.ovf.i8.un":
                case "conv.i8":
                    return CreateInstruction<Conv>(instrukcja, i => i.ConvertType = ConvertType.i8);
                case "conv.r.un":
                    return CreateInstruction<Conv>(instrukcja, i => i.ConvertType = ConvertType.r);
                case "conv.r4":
                    return CreateInstruction<Conv>(instrukcja, i => i.ConvertType = ConvertType.r4);
                case "conv.r8":
                    return CreateInstruction<Conv>(instrukcja, i => i.ConvertType = ConvertType.r8);
                case "conv.ovf.u":
                case "conv.ovf.u.un":
                case "conv.u":
                    return CreateInstruction<Conv>(instrukcja, i => i.ConvertType = ConvertType.u);
                case "conv.ovf.u1":
                case "conv.ovf.u1.un":
                case "conv.u1":
                    return CreateInstruction<Conv>(instrukcja, i => i.ConvertType = ConvertType.u1);
                case "conv.ovf.u2":
                case "conv.ovf.u2.un":
                case "conv.u2":
                    return CreateInstruction<Conv>(instrukcja, i => i.ConvertType = ConvertType.u2);
                case "conv.ovf.u4":
                case "conv.ovf.u4.un":
                case "conv.u4":
                    return CreateInstruction<Conv>(instrukcja, i => i.ConvertType = ConvertType.u4);
                case "conv.ovf.u8":
                case "conv.ovf.u8.un":
                case "conv.u8":
                    return CreateInstruction<Conv>(instrukcja, i => i.ConvertType = ConvertType.u8);




            }
            return null;
        }
    }
}
