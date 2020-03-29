using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Converts
{
    public enum ConvertType
    {
        i4
    }

    //
    public class Conv : InstructionBase
    {
        public ConvertType ConvertType;
        public override void Wykonaj()
        {
            dynamic a = HardwareContext.PopObject();

            switch(this.ConvertType)
            {
                case ConvertType.i4:
                    HardwareContext.PushObject((int)a);
                    break;
            }           

            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}
