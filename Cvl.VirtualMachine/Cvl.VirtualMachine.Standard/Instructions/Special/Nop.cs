using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Special
{  

    /// <summary>
    /// Fills space if opcodes are patched. No meaningful operation is performed although a processing cycle can be consumed.
    /// </summary>
    public class Nop : InstructionBase
    {
        public override void Wykonaj()
        {
            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}
