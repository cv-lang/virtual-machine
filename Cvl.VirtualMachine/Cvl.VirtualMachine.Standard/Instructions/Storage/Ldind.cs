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
    /// load value of type native int into memory at address.
    /// https://en.wikipedia.org/wiki/List_of_CIL_instructions
    /// </summary>
    public class Ldind : InstructionBase
    {

        public override void Wykonaj()
        {
            WykonajNastepnaInstrukcje();
        }
    }
}
