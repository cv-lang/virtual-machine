using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    //Transfers control to a target instruction if value is null.
    public class Brnull : InstructionBase
    {
        public override void Wykonaj()
        {
            var wynik = false;
            dynamic a = PopObject();
            if (a == null)
            {
                wynik = false;
            } 
            else
            {
                wynik = true;
            }


            if (wynik == false)
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