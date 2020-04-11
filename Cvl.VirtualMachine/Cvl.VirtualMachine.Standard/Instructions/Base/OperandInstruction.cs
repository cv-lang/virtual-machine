using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Base
{
    public class OperandInstruction : InstructionBase
    {
        public object Operand { get; set; }
    }
}
