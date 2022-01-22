using Cvl.VirtualMachine.Instructions.Base;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class StobjFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "stobj":
                case "stobj.s":
                    return CreateInstruction<Stobj>(instrukcja);                
            }
            return null;
        }
    }

    /// <summary>
    /// Copies a value of a specified type from the evaluation stack into a supplied memory address.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stobj?view=net-5.0
    /// </summary>
    public class Stobj : IndexedInstruction
    {

        public override void Wykonaj()
        {
            //nic nie robię
            WykonajNastepnaInstrukcje();
        }
    }
}
