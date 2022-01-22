using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Logic
{
    /// <summary>
    /// Negates a value and pushes the result onto the evaluation stack.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.neg
    /// </summary>
    public class Neg : InstructionBase
    {
        public override void Wykonaj()
        {
            dynamic a = PopObject();

            //TODO:// sprawdzić czy napewno to jest to czy nie -
            dynamic wynik = -a;
            PushObject(wynik);
            WykonajNastepnaInstrukcje();
        }
    }
}