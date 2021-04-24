using Cvl.VirtualMachine.Core;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Exceptions
{
    /// <summary>
    /// Rethrows the current exception.
    /// https://docs.microsoft.com/pl-pl/dotnet/api/system.reflection.emit.opcodes.rethrow?view=net-5.0
    /// </summary>
    public class Rethrow : InstructionBase
    {
        public override void Wykonaj()
        {
            HardwareContext.Status = VirtualMachineState.Exception;
            var rzuconyWyjatek = PopObject();
            MethodContext.WirtualnaMaszyna.EventThrowException(rzuconyWyjatek as Exception);

            Throw.ObslugaRzuconegoWyjatku(MethodContext.WirtualnaMaszyna, rzuconyWyjatek);
        }
    }
}
