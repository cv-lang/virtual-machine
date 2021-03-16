using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Logic
{
    public class Or : InstructionBase
    {
        public override void Wykonaj()
        {
            dynamic b = PopObject();
            dynamic a = PopObject();

            dynamic wynik = a | b;
            PushObject(wynik);
            WykonajNastepnaInstrukcje();
        }
    }
}
