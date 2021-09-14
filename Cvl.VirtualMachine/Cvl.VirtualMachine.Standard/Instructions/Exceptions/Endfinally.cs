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
            var isEmpty = HardwareContext.TryCatchStack.IsEmptyTryBlock();

            if(isEmpty == false)
            {               
                if (HardwareContext.ThrowedException != null)
                {
                    //jestem w trakcie wyjątku, przechodzę przez stos do obsługi wyjątku

                    //sciagam ze stosu jeden blok trycatch, bo nie został wywołene zakończenie try -> leave
                    HardwareContext.TryCatchStack.PopTryBlock();

                    Throw.ObslugaRzuconegoWyjatku(HardwareContext.WirtualnaMaszyna, HardwareContext.ThrowedException);
                }
                else
                {
                    WykonajNastepnaInstrukcje();
                }
            }
            else
            {
                WykonajNastepnaInstrukcje();
            }



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
