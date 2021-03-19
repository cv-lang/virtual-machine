using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class LdlenFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "ldlen"://TODO:Testy                 
                    return CreateInstruction<Ldlen>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Pushes the number of elements of a zero-based, one-dimensional array onto the evaluation stack.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldlen
    /// </summary>
    public class Ldlen : InstructionBase
    {

        public override void Wykonaj()
        {
            //pobieram obiekt ze stosu
            var ar = PopObject() as Array;
            var v= ar.Length;
            PushObject(v);

            WykonajNastepnaInstrukcje();
        }
    }

    class ldlen
    {
    }
}
