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
            var rzuconyWyjatek = PopObject();
            if (MethodContext.Status == VirtualMachineState.Exception)
            {
                //jestem w trakcie wyjątku, przechodzę przez stos do obsługi wyjątku
                Throw.ObslugaRzuconegoWyjatku(MethodContext.WirtualnaMaszyna, rzuconyWyjatek);
            }
            else
            {
                WykonajNastepnaInstrukcje();
            }
        }
    }
}
