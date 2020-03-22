using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Base
{
    public class IndexedInstruction: InstructionBase
    {
        public int Index { get; set; }

        public void Inicialize(Instruction instruction, int? index = null)
        {
            base.Inicialize(instruction);

            if(index != null)
            {
                Index = index.Value;
            } else
            {
                if(instruction.Operand is System.Reflection.LocalVariableInfo vr)
                {
                    Index = vr.LocalIndex;
                }
                else if (instruction.Operand is System.Reflection.ParameterInfo parameterInfo)
                {
                    Index = parameterInfo.Position;
                }
                else if (instruction.Operand is Int32 i)
                {
                    Index = i;
                } 
                else
                {
                    throw new Exception($"brak obsugi instrukcji {instruction.Next} dla operanda {instruction.Operand}");
                }
            }   
        }

        protected int GetIndex()
        {
            var index = Index;

            if(Instruction.Operand is System.Reflection.ParameterInfo parametrInfo)
            {
                var methodInfo = (System.Reflection.MethodInfo)parametrInfo.Member;
                if(methodInfo.CallingConvention.HasFlag(System.Reflection.CallingConventions.HasThis))
                {
                    index++;
                }
            }            

            return index;
        }

        public override string ToString()
        {
            return $"{Instruction} {Index}";
        }
    }
}
