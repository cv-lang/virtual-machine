using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class LdftnFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "ldftn":
                    return CreateInstruction<Ldftn>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Pushes an unmanaged pointer (type native int) to the native code implementing a specific method onto the evaluation stack.
    /// https://msdn.microsoft.com/pl-pl/library/system.reflection.emit.opcodes.ldftn(v=vs.110).aspx
    /// </summary>
    public class Ldftn : InstructionBase
    {        
        public override void Wykonaj()
        {
            var methodDefiniton = Instruction.Operand as System.Reflection.MethodInfo;
            
            HardwareContext.PushObject(methodDefiniton.MethodHandle.GetFunctionPointer());
            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}
