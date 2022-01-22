using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class LdnullFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "ldnull":
                    return CreateInstruction<Ldnull>(instrukcja);
            }
            return null;
        }
    }
    public class Ldnull : InstructionBase
    {
        public override void Wykonaj()
        {
            PushObject(null);
            WykonajNastepnaInstrukcje();
        }
    }
}
