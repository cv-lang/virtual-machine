using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    /// <summary>
    /// Pushes a supplied value of type int32 onto the evaluation stack as an int32.
    /// https://msdn.microsoft.com/pl-pl/library/system.reflection.emit.opcodes.ldc_i4(v=vs.110).aspx
    /// </summary>
    public class Ldc : InstructionBase
    {
        public int? ConstValue { get; set; }
        public override void Wykonaj()
        {
            if (ConstValue != null)
            {
                PushObject(ConstValue);
            }
            else
            {
                var constVal = Instruction.Operand;
                PushObject(constVal);
            }
            WykonajNastepnaInstrukcje();
        }
    }

    public class LdcFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "ldc.i4.m1":
                    return CreateInstruction<Ldc>(instrukcja, i => i.ConstValue = -1);
                case "ldc.i4":
                    return CreateInstruction<Ldc>(instrukcja, i => i.ConstValue = (int)instrukcja.Operand);
                case "ldc.i4.0":
                    return CreateInstruction<Ldc>(instrukcja, i => i.ConstValue = 0);
                case "ldc.i4.1":
                    return CreateInstruction<Ldc>(instrukcja, i=> i.ConstValue=1);
                case "ldc.i4.2":
                    return CreateInstruction<Ldc>(instrukcja, i => i.ConstValue =2);                    
                case "ldc.i4.3":
                    return CreateInstruction<Ldc>(instrukcja, i => i.ConstValue =3);
                case "ldc.i4.4":
                    return CreateInstruction<Ldc>(instrukcja, i => i.ConstValue =4);                
                case "ldc.i4.s":
                    return CreateInstruction<Ldc>(instrukcja);
                case "ldc.r8":
                    return CreateInstruction<Ldc>(instrukcja);
            }

            if(instrukcja.OpCode.Name.Contains("ldc.i4"))
            {
                
                var vs = instrukcja.OpCode.Name.Split('.').Last();
                var v = int.Parse(vs);

                return CreateInstruction<Ldc>(instrukcja, i => i.ConstValue = v);
            }


            return null;
        }
    }

    
}
