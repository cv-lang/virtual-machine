using Cvl.VirtualMachine.Instructions.Base;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class LdfldFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "ldfld":
                    return CreateInstruction<Ldfld>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Finds the value of a field in the object whose reference is currently on the evaluation stack.
    /// </summary>
    public class Ldfld : InstructionBase
    {

        public override void Wykonaj()
        {
            //pobieram obiekt ze stosu
            var obj = HardwareContext.PopObject();

            //szukam pola
            var field = (System.Reflection.FieldInfo)Instruction.Operand;
            var val = field.GetValue(obj);

            //kładę na stos wartość pola
            HardwareContext.PushObject(val);

            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}
