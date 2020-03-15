using Cvl.VirtualMachine.Instructions.Base;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class LdargaFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "ldarga.s":
                    return CreateIndexedInstruction<Ldarga>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Load an argument address onto the evaluation stack.
    /// </summary>
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
