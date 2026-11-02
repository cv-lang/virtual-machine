using Cvl.VirtualMachine.Core.Variables;
using Cvl.VirtualMachine.Core.Variables.Values;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    //Transfers control to a target instruction if value is false, a null reference, or zero.
    public class Brfalse : InstructionBase
    {        
        public override void Wykonaj()
        {
            if (CzyPrawda(PopObject()) == false)
            {
                var op = Instruction.Operand as Instruction;
                var nextOffset = op.Offset;
                WykonajSkok(nextOffset);
            }
            else
            {
                WykonajNastepnaInstrukcje();
            }
        }
    }
}