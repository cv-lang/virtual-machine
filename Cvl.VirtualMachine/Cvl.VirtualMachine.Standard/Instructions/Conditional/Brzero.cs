using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    //Transfers control to a target instruction if value is zero.
    public class Brzero : InstructionBase
    {
        public override void Wykonaj()
        {
            var wynik = false;
            dynamic a = PopObject();
            if (a == 0)
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