using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Reflection;

namespace Cvl.VirtualMachine.Instructions.Casts
{
    public class CastsInstructionsFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "castclass":
                    return CreateInstruction<Castclass>(instrukcja);
            }
            return null;
        }
    }
}
