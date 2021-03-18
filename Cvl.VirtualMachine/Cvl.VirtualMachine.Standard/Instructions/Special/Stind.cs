using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class StindFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "stind"://TODO:Testy 
                case "stind.i":
                case "stind.i2":
                case "stind.i4":
                case "stind.i8":
                case "stind.r4":
                case "stind.r8":
                case "stind.ref":
                    return CreateInstruction<Stind>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Loads the element at a specified array index onto the top of the evaluation stack as the type specified in the instruction.
    /// https://docs.microsoft.com/pl-pl/dotnet/api/system.reflection.emit.opcodes.ldelem?view=net-5.0
    /// </summary>
    public class Stind : InstructionBase
    {

        public override void Wykonaj()
        {
            WykonajNastepnaInstrukcje();
        }
    }
}
