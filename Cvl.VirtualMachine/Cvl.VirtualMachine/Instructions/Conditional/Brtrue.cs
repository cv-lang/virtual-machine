using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    //Transfers control to a target instruction (short form) if value is true, not null, or non-zero.
    public class Brtrue : InstructionBase
    {      
        public override void Wykonaj()
        {
            var wynik = false;
            dynamic a = HardwareContext.PopObject();
            if (a == null)
            {
                wynik = false;
            }
            else if (a is bool)
            {
                wynik = (bool)a;
            }
            else if (a is int)
            {
                wynik = a == 1;
            }
            else
            {
                wynik = true;
            }

            
            if (wynik)
            {
                var op = Instruction.Operand as Instruction;
                var nextOffset = op.Offset;
                HardwareContext.WykonajSkok(nextOffset);
            }
            else
            {
                HardwareContext.WykonajNastepnaInstrukcje();
            }
        }
    }
}