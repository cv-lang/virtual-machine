using Cvl.VirtualMachine.Core.Variables.Addresses;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class LdfldaFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "ldflda":
                    return CreateInstruction<Ldflda>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Finds the address of a field in the object whose reference is currently on the evaluation stack.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldflda
    /// </summary>
    public class Ldflda : InstructionBase
    {

        public override void Wykonaj()
        {
            //pobieram obiekt ze stosu
            var obj = PopObject();

            //szukam pola
            var field = (System.Reflection.FieldInfo)Instruction.Operand;
            
            var fieldAddress = new FieldAddress();
            fieldAddress.Field = field;
            fieldAddress.Object = obj;

            //kładę na stos wartość pola
            Push(fieldAddress);

            WykonajNastepnaInstrukcje();
        }
    }
}
