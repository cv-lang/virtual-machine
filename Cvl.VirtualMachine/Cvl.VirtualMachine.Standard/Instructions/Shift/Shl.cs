﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Shift
{
    class Shl : InstructionBase
    {
        public override void Wykonaj()
        {
            dynamic b = PopObject();
            dynamic a = PopObject();

            dynamic wynik = a << b;
            PushObject(wynik);
            WykonajNastepnaInstrukcje();
        }
    }
}
