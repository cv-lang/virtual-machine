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
                case "ldarg.s":
                    return CreateIndexedInstruction<Ldarg>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Loads an argument (referenced by a specified index value) onto the stack.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_s?view=netcore-3.1
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
