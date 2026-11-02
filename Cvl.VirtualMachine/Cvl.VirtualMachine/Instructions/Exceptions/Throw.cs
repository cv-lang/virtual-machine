using Cvl.VirtualMachine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Exceptions
{
    public class Throw : InstructionBase
    {
        public override void Wykonaj()
        {
            HardwareContext.Status = VirtualMachineState.Exception;
            var rzuconyWyjatek = PopObject() as Exception ?? new NullReferenceException();
            MethodContext.WirtualnaMaszyna.EventThrowException(rzuconyWyjatek);

            HardwareContext.ExceptionHandling.ObslugaRzuconegoWyjatku(rzuconyWyjatek);
        }
    }
}
