using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class LdsfldFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "ldsfld":
                    return CreateInstruction<Ldsfld>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Pushes the value of a static field onto the evaluation stack.
    /// https://msdn.microsoft.com/pl-pl/library/system.reflection.emit.opcodes.ldsfld(v=vs.110).aspx
    /// </summary>
    public class Ldsfld : InstructionBase
    {        
        public override void Wykonaj()
        {
            //pobieram statyczną zmienną
            var fieldDefinition = Instruction.Operand as System.Reflection.FieldInfo;
            var val = fieldDefinition.GetValue(null);
            //var typ = fieldDefinition.DeclaringType;//.GetSystemType();
            //var field = typ.GetField(fieldDefinition.Name);
            //var val = field.GetValue(null);

            HardwareContext.PushObject(val);
            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}
