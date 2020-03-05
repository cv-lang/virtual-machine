using Cvl.VirtualMachine.Instructions.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class Ldloca : OperandInstruction
    {
        public override void Wykonaj()
        {
            var vr = Operand as System.Reflection.LocalVariableInfo;

            var o = PobierzAdresZmiennejLokalnej(vr.LocalIndex);
            Push(o);
            WykonajNastepnaInstrukcje();
        }

        
    }
}
