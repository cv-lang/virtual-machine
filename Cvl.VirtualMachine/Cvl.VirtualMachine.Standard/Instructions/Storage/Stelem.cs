using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class StelemFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "stelem"://TODO:Testy 
                case "stelem.i":
                case "stelem.i2":
                case "stelem.i4":
                case "stelem.i8":
                case "stelem.r4":
                case "stelem.r8":
                case "stelem.ref":
                    return CreateInstruction<Stelem>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Loads the element at a specified array index onto the top of the evaluation stack as the type specified in the instruction.
    /// https://docs.microsoft.com/pl-pl/dotnet/api/system.reflection.emit.opcodes.ldelem?view=net-5.0
    /// </summary>
    public class Stelem : InstructionBase
    {

        public override void Wykonaj()
        {
            //pobieram obiekt ze stosu
            var index = (int)PopObject();
            var ar = PopObject() as Array;
            var v = PopObject();
            ar.SetValue(v, index);

            WykonajNastepnaInstrukcje();
        }
    }
}
