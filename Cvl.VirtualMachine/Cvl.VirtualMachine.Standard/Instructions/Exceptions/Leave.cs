using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Exceptions
{
    /// <summary>
    /// Exits a protected region of code, unconditionally transferring control to a specific target instruction.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.leave?view=netcore-3.1
    /// </summary>
    public class Leave : InstructionBase
    {
        public override void Wykonaj()
        {
            //TODO: tu trzeba dodać jeszcze obsługę finall w bloku try..catch, bo ona może wyskoczyć ponad to

            var i = Instruction.Operand as Instruction;
            var nextOffset = i.Offset;
            WykonajSkok(nextOffset);
        }
    }
}
