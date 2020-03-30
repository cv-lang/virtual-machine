using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class StsfldFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "stsfld":
                    return CreateInstruction<Stsfld>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Replaces the value of a static field with a value from the evaluation stack.
    /// https://msdn.microsoft.com/pl-pl/library/system.reflection.emit.opcodes.stsfld(v=vs.110).aspx
    /// </summary>
    public class Stsfld : InstructionBase
    {
        public override void Wykonaj()
        {
            var o = HardwareContext.PopObject();

            //ustawiam statyczną zmienną wartością ze stosu
            var fieldDefinition = Instruction.Operand as FieldInfo;

            //var typ = fieldDefinition.DeclaringType.GetSystemType();
            // var field = typ.GetField(fieldDefinition.Name);
            fieldDefinition.SetValue(null, o);

            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}
