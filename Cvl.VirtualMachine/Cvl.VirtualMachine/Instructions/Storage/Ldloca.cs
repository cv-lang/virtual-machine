using Cvl.VirtualMachine.Instructions.Base;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class LdlocaFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "ldloca.s":
                    return CreateIndexedInstruction<Ldloca>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Loads the address of the local variable at a specific index onto the evaluation stack.
    /// </summary>
    public class Ldloca : IndexedInstruction
    {
        public override void Wykonaj()
        {            
            var o = HardwareContext.PobierzAdresZmiennejLokalnej(Index);
            HardwareContext.Push(o);
            HardwareContext.WykonajNastepnaInstrukcje();
        }        
    }
}
