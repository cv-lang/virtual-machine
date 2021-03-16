using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Shift
{
    class Shr : InstructionBase
    {
        public override void Wykonaj()
        {
            dynamic a = PopObject();
            dynamic b = PopObject();

            dynamic wynik = a >> b;
            PushObject(wynik);
            WykonajNastepnaInstrukcje();
        }
    }
}
