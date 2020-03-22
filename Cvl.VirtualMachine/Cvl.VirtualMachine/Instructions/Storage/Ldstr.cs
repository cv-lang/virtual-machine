using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class LdstrFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "ldstr":
                    return CreateInstruction<Ldstr>(instrukcja);
            }
            return null;
        }
    }

    public class Ldstr : InstructionBase
    {        
        public override void Wykonaj()
        {
            string str = Instruction.Operand as string;
            HardwareContext.PushObject(str);
            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}
