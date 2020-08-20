using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Logic
{
    /// <summary>
    /// Negates a value and pushes the result onto the evaluation stack.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.neg?view=netcore-3.1
    /// </summary>
    public class Neg : InstructionBase
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