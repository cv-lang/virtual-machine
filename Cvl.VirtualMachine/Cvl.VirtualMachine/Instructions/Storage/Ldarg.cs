using Cvl.VirtualMachine.Instructions.Base;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class LdargFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "ldarg.0":
                    return CreateIndexedInstruction<Ldarg>(instrukcja, 0);
                case "ldarg.1":
                    return CreateIndexedInstruction<Ldarg>(instrukcja, 1);
                case "ldarg.2":
                    return CreateIndexedInstruction<Ldarg>(instrukcja, 2);
                case "ldarg.3":
                    return CreateIndexedInstruction<Ldarg>(instrukcja, 3);                
            }
            return null;
        }
    }

    /// <summary>
    /// Loads an argument (referenced by a specified index value) onto the stack.
    /// </summary>
    public class Ldarg : IndexedInstruction
    {
        public override void Wykonaj()
        {
            var o = PobierzLokalnyArgument(Index);
            PushObject(o);
            WykonajNastepnaInstrukcje();
        }

        
    }
}
