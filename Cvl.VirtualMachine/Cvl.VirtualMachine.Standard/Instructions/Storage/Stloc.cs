using Cvl.VirtualMachine.Instructions.Base;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class StlocFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "stloc":
                case "stloc.s":
                    return CreateIndexedInstruction<Stloc>(instrukcja);
                case "stloc.0":
                    return CreateIndexedInstruction<Stloc>(instrukcja, 0);
                case "stloc.1":
                    return CreateIndexedInstruction<Stloc>(instrukcja, 1);
                case "stloc.2":
                    return CreateIndexedInstruction<Stloc>(instrukcja, 2);
                case "stloc.3":
                    return CreateIndexedInstruction<Stloc>(instrukcja, 3);
            }
            return null;
        }
    }

    /// <summary>
    /// Pops the current value from the top of the evaluation stack and stores it in a the local variable list at a specified index.
    /// </summary>
    public class Stloc : IndexedInstruction
    {
        
        public override void Wykonaj()
        {
            var o = HardwareContext.PopObject();

            var a = Instruction.Operand as System.Reflection.LocalVariableInfo;
            if (a != null)
            {
                HardwareContext.ZapiszLokalnaZmienna(o, a.LocalIndex);
            }
            else
            {
                HardwareContext.ZapiszLokalnaZmienna(o, Index);
            }
            HardwareContext.WykonajNastepnaInstrukcje();
        }        
    }
}
