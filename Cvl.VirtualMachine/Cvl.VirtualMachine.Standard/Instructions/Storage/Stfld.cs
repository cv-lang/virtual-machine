using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class StfldFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "stfld":
                    return CreateInstruction<Stfld>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Replaces the value stored in the field of an object reference or pointer with a new value.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stfld?view=netframework-4.8
    /// </summary>
    public class Stfld : InstructionBase
    {    
        
        public override void Wykonaj()
        {
            //pobieram vartosc ze stosu
            var objVal = PopObject();
            var obj = PopObject();

            var field = (System.Reflection.FieldInfo)Instruction.Operand;
            field.SetValue(obj, objVal);
            
            WykonajNastepnaInstrukcje();
        }
    }
}
