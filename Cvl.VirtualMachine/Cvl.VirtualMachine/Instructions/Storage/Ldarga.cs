using Cvl.VirtualMachine.Instructions.Base;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class LdargaFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "ldarga.s":
                    return CreateIndexedInstruction<Ldarga>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Load an argument address onto the evaluation stack.
    /// </summary>
    public class Ldarga : IndexedInstruction
    {
        public override void Wykonaj()
        {
            var index = Index;
            if (((System.Reflection.MethodInfo)
                ((System.Reflection.ParameterInfo)Instruction.Operand).Member)
                .CallingConvention.HasFlag(System.Reflection.CallingConventions.HasThis))
            {
                index++;
            }

            var o = HardwareContext.PobierzAdresArgumentu(index);
            HardwareContext.Push(o);
            HardwareContext.WykonajNastepnaInstrukcje();
        }


    }
}
