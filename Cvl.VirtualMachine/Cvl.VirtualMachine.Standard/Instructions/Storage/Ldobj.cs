using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class LdobjFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "ldobj":
                    return CreateInstruction<Ldobj>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Kopiuje obiekt typu wartości wskazywany przez adres na początku stosu oceny.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldobj?view=net-5.0
    /// </summary>
    public class Ldobj : InstructionBase
    {
        public override void Wykonaj()
        {
            //TODO: napisać test
            //tu nic nie musimy robić - C# sam sobie to obsłuży 
            WykonajNastepnaInstrukcje();
        }
    }    
}
