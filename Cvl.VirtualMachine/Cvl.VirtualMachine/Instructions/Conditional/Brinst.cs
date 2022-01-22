using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    //Transfers control to a target instruction (short form) if value is not null.
    public class Brinst : InstructionBase
    {
        public override void Wykonaj()
        {
            var wynik = false;
            dynamic a = PopObject();
            if (a != null)
            {
                wynik = true;
            } 
            else
            {
                wynik = false;
            }


            if (wynik)
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