using Cvl.VirtualMachine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Exceptions
{
    public class Endfinally : InstructionBase
    {
        public override void Wykonaj()
        {
            HardwareContext.ExceptionHandling.FromEndFinally();

            //if (HardwareContext.AktualnaMetoda.NumerWykonywanejInstrukcji.ExecutionPointsStack.Count > 1)
            //{
            //    HardwareContext.AktualnaMetoda.NumerWykonywanejInstrukcji.Pop();
            //} else
            //{
            //    WykonajNastepnaInstrukcje();
            //}

            //var isEmpty = HardwareContext.TryCatchStack.IsEmptyTryBlock();

            //if(isEmpty == false)
            //{
            //    ////sciagam ze stosu jeden blok trycatch
            //    //HardwareContext.TryCatchStack.PopTryBlock();
            //    HardwareContext.AktualnaMetoda.NumerWykonywanejInstrukcji.Pop();

            //    if (HardwareContext.ThrowedException != null)
            //    {
            //        //jestem w trakcie wyjątku, przechodzę przez stos do obsługi wyjątku

            //        //sciagam ze stosu jeden blok trycatch, bo nie został wywołene zakończenie try -> leave
            //        HardwareContext.TryCatchStack.PopTryBlock();

            //        Throw.ObslugaRzuconegoWyjatku(HardwareContext.WirtualnaMaszyna, HardwareContext.ThrowedException);
            //    }
            //    else
            //    {
            //        //jesteśmy w miejscu bez wyjątku, 

            //        if(HardwareContext.TryCatchStack.PeekTryBlock().ExceptionHandlingClause.Flags == System.Reflection.ExceptionHandlingClauseOptions.Finally)
            //        {
            //            //jestemy bez wyjątku i w finally
            //            HardwareContext.TryCatchStack.PopTryBlock();
            //        }

            //        WykonajNastepnaInstrukcje();
            //    }
            //}
            //else
            //{
            //    WykonajNastepnaInstrukcje();
            //}



            //var rzuconyWyjatek = PopObject();
            //if (HardwareContext.Status == VirtualMachineState.Exception)
            //{
            //    //jestem w trakcie wyjątku, przechodzę przez stos do obsługi wyjątku
            //    Throw.ObslugaRzuconegoWyjatku(MethodContext.WirtualnaMaszyna, rzuconyWyjatek);
            //}
            //else
            //{
            //    WykonajNastepnaInstrukcje();
            //}
        }
    }
}
