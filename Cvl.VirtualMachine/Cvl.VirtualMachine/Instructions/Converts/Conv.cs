using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Converts
{
    public enum ConvertType
    {
        i4,
        r8
    }

    /// <summary>
    /// Converts the value on top of the evaluation stack to ...
    /// </summary>
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
                case ConvertType.r8:
                    HardwareContext.PushObject((double)a);
                    break;
            }           

            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}
