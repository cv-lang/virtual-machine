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
            var wynik = false;            
            dynamic a = PopObject();
            
            if (a == null)
            {
                wynik = true;
            }
            else if (a is bool b)
            {
                wynik = b == false;
            }
            else if (a is int i)
            {
                wynik = i == 0;
            }
            else
            {
                wynik = false;
            }

            
            if (wynik == true)
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