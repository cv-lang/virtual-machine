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
                    //Position nie liczy this - w metodzie instancji (i konstruktorze) argument 0 to this
                    var metoda = (System.Reflection.MethodBase)parameterInfo.Member;
                    Index = parameterInfo.Position + (metoda.CallingConvention.HasFlag(System.Reflection.CallingConventions.HasThis) ? 1 : 0);
                }
                else if (instruction.Operand is Int32 i)
                {
                    Index = i;
                } 
                else
                {
                    throw new Exception($"brak obsugi instrukcji {instruction} dla operanda {instruction.Operand}");
                }
            }   
        }

        protected int GetIndex()
        {
            //this jest już w Index (Inicialize) - drugi raz przesuwał ldarga.s o jeden argument dalej
            return Index;
        }

        public override string ToString()
        {
            return $"{Instruction} {Index}";
        }
    }
}
