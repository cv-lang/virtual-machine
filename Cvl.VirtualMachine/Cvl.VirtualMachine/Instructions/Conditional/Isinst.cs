using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    public class Isinst : InstructionBase
    {        
        public override void Wykonaj()
        {
            dynamic b = HardwareContext.PopObject();
            if (b != null)
            {
                var typ = b.GetType();
                var typOperanda = (Type)Instruction.Operand;
                if (typOperanda.IsAssignableFrom(typ))
                {
                    //mamy ten sam typ
                    HardwareContext.PushObject(b);
                }
                else
                {
                    HardwareContext.PushObject(null);
                }
            }
            else
            {
                HardwareContext.PushObject(null);
            }

            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}