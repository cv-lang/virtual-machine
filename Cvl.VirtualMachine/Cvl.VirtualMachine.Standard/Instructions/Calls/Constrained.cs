using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Calls
{
    /// <summary>
    /// Constrains the type on which a virtual method call is made.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.constrained?view=netframework-4.8
    /// </summary>
    public class Constrained : InstructionBase
    {        
        public override void Wykonaj()
        {
            HardwareContext.ConstrainedType = (Type)this.Instruction.Operand;
            //throw new NotImplementedException();
            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}
