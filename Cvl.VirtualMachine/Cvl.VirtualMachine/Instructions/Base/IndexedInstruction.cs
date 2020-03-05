using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Base
{
    public class IndexedInstruction: InstructionBase
    {
        public int Index { get; set; }
    }
}
