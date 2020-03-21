using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions
{
    public class InstructionBase
    {
        internal Instruction Instruction { get; set; }
        public HardwareContext HardwareContext { get; set; }
        
        public virtual void Wykonaj()
        {
        }

        public void Inicialize(Instruction instruction)
        {
            Instruction = instruction;
        }

        protected virtual void InstructionInicialize()
        {

        }

    }
}
