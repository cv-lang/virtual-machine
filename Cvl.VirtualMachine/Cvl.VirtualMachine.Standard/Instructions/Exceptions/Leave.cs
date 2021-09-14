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
            var block = HardwareContext.TryCatchStack.PopTryBlock();

            if(block.ExceptionHandlingClause.Flags == System.Reflection.ExceptionHandlingClauseOptions.Clause)
            {
                //jestem w catchu, na jego końcu

                HardwareContext.ThrowedException = null;

                //przechodze do instrukcji poza blokiem try
                var i = Instruction.Operand as Instruction;
                var nextOffset = i.Offset;
                WykonajSkok(nextOffset);
            } else if (block.ExceptionHandlingClause.Flags == System.Reflection.ExceptionHandlingClauseOptions.Finally)
            {
                //jestem na końcu try
                //przechodzę do finally                
                var nextOffset = block.ExceptionHandlingClause.HandlerOffset;
                WykonajSkok(nextOffset);
            }    


        }
    }
}
