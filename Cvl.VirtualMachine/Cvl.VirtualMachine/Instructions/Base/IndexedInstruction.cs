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

                var vr = instruction.Operand as System.Reflection.LocalVariableInfo;
                Index = vr.LocalIndex;
            }
            
            
        }
    }
}
