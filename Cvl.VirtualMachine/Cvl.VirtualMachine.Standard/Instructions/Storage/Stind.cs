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
                case "stind.i1":
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
    ///Store value of type native int into memory at address.
    /// https://en.wikipedia.org/wiki/List_of_CIL_instructions
    /// </summary>
    public class Stind : InstructionBase
    {

        public override void Wykonaj()
        {
            WykonajNastepnaInstrukcje();
        }
    }
}
