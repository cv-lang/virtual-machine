using System;
using System.Collections.Generic;
using System.Text;
using Mono.Reflection;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    public class LdtokenFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "ldtoken":
                    return CreateInstruction<Ldtoken>(instrukcja);
            }
            return null;
        }
    }

    /// <summary>
    /// Converts a metadata token to its runtime representation, pushing it onto the evaluation stack.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldtoken
    /// </summary>
    public class Ldtoken : InstructionBase
    {
        public override void Wykonaj()
        {
            if (Instruction.Operand is Type typ)
            {
                HardwareContext.PushObject(typ.TypeHandle);
            } 
            else if(Instruction.Operand is System.Reflection.FieldInfo fieldInfo)             
            {
                HardwareContext.PushObject(fieldInfo.FieldHandle);                
            } 
            else
            {
                throw new NotImplementedException($"Ldtoken: operand in not known type {Instruction.Operand.ToString()}");
            }
            HardwareContext.WykonajNastepnaInstrukcje();
        }   
    }
}
