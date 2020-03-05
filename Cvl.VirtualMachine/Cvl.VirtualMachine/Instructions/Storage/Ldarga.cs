using Cvl.VirtualMachine.Instructions.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class Ldarga : IndexedInstruction
    {
        protected override void InstructionInicialize()
        {
            throw new NotImplementedException("instrukcja Ldarga");
            //Index = ((Mono.Cecil.ParameterDefinition)Instruction.Operand).Index;
            //if (((Mono.Cecil.ParameterDefinition)instrukcja.Operand).Method.HasThis)
            //{
            //    index++;
            //}

            //this.instrukcja = instrukcja;
        }

        public override void Wykonaj()
        {
            var o = PobierzAdresArgumentu(Index);
            Push(o);
            WykonajNastepnaInstrukcje();
        }


    }
}
