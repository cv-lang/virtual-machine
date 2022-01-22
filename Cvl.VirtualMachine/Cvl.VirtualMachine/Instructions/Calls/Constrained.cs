﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Calls
{
    /// <summary>
    /// Constrains the type on which a virtual method call is made.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.constrained?view=netframework-4.8
    /// https://www.ecma-international.org/wp-content/uploads/ECMA-335_6th_edition_june_2012.pdf page 342.
    /// </summary>
    public class Constrained : InstructionBase
    {        
        public override void Wykonaj()
        {
            MethodContext.ConstrainedType = (Type)this.Instruction.Operand;
            //throw new NotImplementedException();
            WykonajNastepnaInstrukcje();
        }
    }
}
