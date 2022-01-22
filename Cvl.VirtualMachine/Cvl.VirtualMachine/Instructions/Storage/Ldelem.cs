using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class LdelemFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "ldelem"://TODO:Testy 
                case "ldelem.i":
                case "ldelem.i2":
                case "ldelem.i4":
                case "ldelem.i8":
                case "ldelem.r4":
                case "ldelem.r8":
                case "ldelem.ref":
                case "ldelem.u1":
                case "ldelem.u2":
                case "ldelem.u4":
                case "ldelem.u8":
                    return CreateInstruction<Ldelem>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Loads the element at a specified array index onto the top of the evaluation stack as the type specified in the instruction.
    /// https://docs.microsoft.com/pl-pl/dotnet/api/system.reflection.emit.opcodes.ldelem?view=net-5.0
    /// </summary>
    public class Ldelem : InstructionBase
    {

        public override void Wykonaj()
        {
            //pobieram obiekt ze stosu
            var index = (int)PopObject();
            var ar = PopObject() as Array;

            var v = ar.GetValue(index);
            PushObject(v);

            WykonajNastepnaInstrukcje();
        }
    }
}
