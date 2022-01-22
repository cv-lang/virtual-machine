using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Logic
{
    /// <summary>
    /// Computes the bitwise complement of the integer value on top of the stack and pushes the result onto the evaluation stack as the same type.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.not?view=netcore-3.1
    /// </summary>
    public class Not : InstructionBase
    {
        public override void Wykonaj()
        {
            dynamic a = PopObject();

            dynamic wynik = !a;
            PushObject(wynik);
            WykonajNastepnaInstrukcje();
        }
    }
}
