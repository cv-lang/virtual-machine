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
            var block = HardwareContext.TryCatchStack.PeekTryBlock();

            if(block.ExceptionHandlingClause.Flags == System.Reflection.ExceptionHandlingClauseOptions.Clause)
            {
                HardwareContext.TryCatchStack.PopTryBlock(); //w catchu zdejmuje blok ze stosu
                //jestem w catchu, na jego końcu

                HardwareContext.ThrowedException = null;

                //przechodze do instrukcji poza blokiem try
                var i = Instruction.Operand as Instruction;
                var nextOffset = i.Offset;
                WykonajSkok(nextOffset);
            } else if (block.ExceptionHandlingClause.Flags == System.Reflection.ExceptionHandlingClauseOptions.Finally)
            {

                //jestem na końcu try
                //przechodzę do finally, bolk zostanie zdjety ze stosu w endfinally                
                var nextOffset = block.ExceptionHandlingClause.HandlerOffset;
                WykonajSkok(nextOffset);
            }    


        }
    }
}
