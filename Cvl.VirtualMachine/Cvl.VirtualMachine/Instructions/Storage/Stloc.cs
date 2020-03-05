using Cvl.VirtualMachine.Instructions.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{

    /// <summary>
    /// Pops the current value from the top of the evaluation stack and stores it in a the local variable list at a specified index.
    /// </summary>
    public class Stloc : IndexedInstruction
    {
        
        public override void Wykonaj()
        {
            var o = PopObject();

            var a = Instruction.Operand as System.Reflection.LocalVariableInfo;
            if (a != null)
            {
                ZapiszLokalnaZmienna(o, a.LocalIndex);
            }
            else
            {
                ZapiszLokalnaZmienna(o, Index);
            }
            WykonajNastepnaInstrukcje();
        }

        
    }
}
