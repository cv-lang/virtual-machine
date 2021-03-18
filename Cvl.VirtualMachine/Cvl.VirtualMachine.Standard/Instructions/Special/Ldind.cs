using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class LdindFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "ldind"://TODO:Testy 
                case "ldind.i":
                case "ldind.i2":
                case "ldind.i4":
                case "ldind.i8":
                case "ldind.r4":
                case "ldind.r8":
                case "ldind.ref":
                case "ldind.u1":
                case "ldind.u2":
                case "ldind.u4":
                case "ldind.u8":
                    return CreateInstruction<Ldind>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Loads the element at a specified array index onto the top of the evaluation stack as the type specified in the instruction.
    /// https://docs.microsoft.com/pl-pl/dotnet/api/system.reflection.emit.opcodes.ldelem?view=net-5.0
    /// </summary>
    public class Ldind : InstructionBase
    {

        public override void Wykonaj()
        {
            WykonajNastepnaInstrukcje();
        }
    }
}
