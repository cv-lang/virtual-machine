using Cvl.VirtualMachine.Instructions.Base;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class StargFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "starg.s":
                    return CreateIndexedInstruction<Starg>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Stores the value on top of the evaluation stack in the argument slot at a specified index.
    /// </summary>
    public class Starg : IndexedInstruction
    {              
        public override void Wykonaj()
        {
            var o = PopObject();
            ZapiszLokalnyArgument(o, Index);
            WykonajNastepnaInstrukcje();
        }        
    }
}
