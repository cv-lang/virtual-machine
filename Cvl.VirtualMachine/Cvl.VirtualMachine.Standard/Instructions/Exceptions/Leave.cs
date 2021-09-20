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
            var i = Instruction.Operand as Instruction;
            var nextOffset = i.Offset;

            HardwareContext.ExceptionHandling.FromLeave(nextOffset);

            //var block = HardwareContext.AktualnaMetoda.TryCatchStack.PeekTryBlock();

            
            //if(block.ExceptionHandlingClause.Flags == System.Reflection.ExceptionHandlingClauseOptions.Clause)
            //{
            //    HardwareContext.AktualnaMetoda.TryCatchStack.PopTryBlock(); //w catchu zdejmuje blok ze stosu
            //    //jestem w catchu, na jego końcu

            //    HardwareContext.ThrowedException = null;

            //    //zapisuje obecny punkt wykonania
            //    var currentInstructionIndex = HardwareContext.AktualnaMetoda.NumerWykonywanejInstrukcji.CurrentInstructionIndex;
                

                

            //    //miejsce docelowe, do którego skaczemy
            //    var instructionIndex = HardwareContext.AktualnaMetoda.PobierzNumerInstrukcjiZOffsetem(nextOffset);
            //    HardwareContext.AktualnaMetoda.NumerWykonywanejInstrukcji.SetCurrentInstructionIndex(instructionIndex);

            //    //przechodzę po blokach try..catch..finally
            //    while(HardwareContext.AktualnaMetoda.TryCatchStack.IsEmptyTryBlock() == false)
            //    {
            //        var blok = HardwareContext.AktualnaMetoda.TryCatchStack.PeekTryBlock();
            //        if(blok.MethodFullName != HardwareContext.AktualnaMetoda.MethodFullName)
            //        {
            //            break;
            //        }

            //        if(blok.ExceptionHandlingClause.TryOffset + blok.ExceptionHandlingClause.TryLength < nextOffset)
            //        {
            //            //przeskakujem ten blok, wrzucam na stos
            //            var handlerIndex = HardwareContext.AktualnaMetoda.PobierzNumerInstrukcjiZOffsetem(blok.ExceptionHandlingClause.HandlerOffset);
            //            HardwareContext.AktualnaMetoda.NumerWykonywanejInstrukcji.PushExecutionPoint(handlerIndex);

                        
            //        } else
            //        {
            //            //skok nie przeskakuje tego bloku - nie wychodzi z niego
            //            //tu kończymy 
            //            break;
            //        }

            //        HardwareContext.AktualnaMetoda.TryCatchStack.PopTryBlock();
            //    }
                


            //    //WykonajSkok(nextOffset);
            //} else if (block.ExceptionHandlingClause.Flags == System.Reflection.ExceptionHandlingClauseOptions.Finally)
            //{

            //    //jestem na końcu try
            //    //przechodzę do finally, bolk zostanie zdjety ze stosu w endfinally                
            //    var nextOffset = block.ExceptionHandlingClause.HandlerOffset;
            //    WykonajSkok(nextOffset);
            //}    


        }
    }
}
