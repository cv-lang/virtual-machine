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
    /// Zastępuje element Array w danym indeksie wartością na stosie ewaluacyjnym, którego typ jest określony w instrukcji.
    /// https://docs.microsoft.com/pl-pl/dotnet/api/system.reflection.emit.opcodes.Stelem?view=net-5.0
    /// </summary>
    public class Stelem : InstructionBase
    {

        public override void Wykonaj()
        {
            //pobieram obiekt ze stosu
            var v = PopObject();
            var index = (int)PopObject();
            var ar = PopObject() as Array;
           
            ar.SetValue(v, index);

            WykonajNastepnaInstrukcje();
        }
    }
}
