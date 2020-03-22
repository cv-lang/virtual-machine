using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class LdcFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            Ldc inst = null;
            switch (instrukcja.OpCode.Name)
            {
                case "ldc.i4.1":
                    inst = CreateInstruction<Ldc>(instrukcja);
                    inst.ConstValue = 1;
                    return inst;
                case "ldc.i4.2":
                    inst = CreateInstruction<Ldc>(instrukcja);
                    inst.ConstValue = 2;
                    return inst;
                case "ldc.i4.3":
                    inst = CreateInstruction<Ldc>(instrukcja);
                    inst.ConstValue = 3;
                    return inst;
            }
            return null;
        }
    }

    /// <summary>
    /// Pushes a supplied value of type int32 onto the evaluation stack as an int32.
    /// https://msdn.microsoft.com/pl-pl/library/system.reflection.emit.opcodes.ldc_i4(v=vs.110).aspx
    /// </summary>
    public class Ldc : InstructionBase
    {   
        public int ConstValue { get; set; }
        public override void Wykonaj()
        {
            HardwareContext.PushObject(ConstValue);
            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}
