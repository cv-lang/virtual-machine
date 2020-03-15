using Cvl.VirtualMachine.Instructions.Base;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class LdlocFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "ldloc.s":
                    return CreateIndexedInstruction<Ldloc>(instrukcja);
                case "ldloc.0":
                    return CreateIndexedInstruction<Ldloc>(instrukcja, 0);
                case "ldloc.1":
                    return CreateIndexedInstruction<Ldloc>(instrukcja, 1);
                case "ldloc.2":
                    return CreateIndexedInstruction<Ldloc>(instrukcja, 2);
                case "ldloc.3":
                    return CreateIndexedInstruction<Ldloc>(instrukcja, 3);
            }
            return null;
        }
    }

    /// <summary>
    /// Loads the local variable at a specific index onto the evaluation stack.
    /// </summary>
    public class Ldloc : IndexedInstruction
    {       

        public override void Wykonaj()
        {

            var a = Instruction.Operand as System.Reflection.LocalVariableInfo;
            if (a != null)
            {
                var o = PobierzLokalnaZmienna(a.LocalIndex);
                PushObject(o);
            }
            else
            {
                var o = PobierzLokalnaZmienna(Index);
                PushObject(o);
            }


            WykonajNastepnaInstrukcje();
        }

        
    }
}
