using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Converts
{
    public enum ConvertType
    {
        i4,
        r8,
        i8,
        i,
        i1,
        i2,
        r,
        r4,
        u8,
        u4,
        u2,
        u1,
        u
    }

    /// <summary>
    /// Converts the value on top of the evaluation stack to ...
    /// </summary>
    public class Conv : InstructionBase
    {
        public ConvertType ConvertType;
        public override void Wykonaj()
        {
            dynamic a = PopObject();

            switch(this.ConvertType)
            {
                case ConvertType.i:
                    PushObject((int)a);
                    break;
                case ConvertType.i1:
                    PushObject((SByte)a);
                    break;
                case ConvertType.i2:
                    PushObject((short)a);
                    break;
                case ConvertType.i4:
                    PushObject((int)a);
                    break;
                case ConvertType.i8:
                    PushObject((int)a);
                    break;
                case ConvertType.r:
                    PushObject((float)a);
                    break;
                case ConvertType.r4:
                    PushObject((float)a);
                    break;
                case ConvertType.r8:
                    PushObject((double)a);
                    break;
                case ConvertType.u:
                    PushObject((uint)a);
                    break;
                case ConvertType.u1:
                    PushObject((byte)a);
                    break;
                case ConvertType.u2:
                    PushObject((ushort)a);
                    break;
                case ConvertType.u4:
                    PushObject((UInt32)a);
                    break;
                case ConvertType.u8:
                    PushObject((UInt64)a);
                    break;

            }           

            WykonajNastepnaInstrukcje();
        }
    }
}
