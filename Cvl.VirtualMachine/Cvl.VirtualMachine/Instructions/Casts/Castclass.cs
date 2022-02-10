using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cvl.VirtualMachine.Instructions.Casts
{
    /// <summary>
    /// Attempts to cast an object passed by reference to the specified class.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.castclass?view=net-6.0
    /// </summary>
    public class Castclass : InstructionBase
    {
        public override void Wykonaj()
        {
            dynamic a = PopObject();

            var type = (Type)this.Instruction.Operand;
            if (a == null)
            {
                PushObject(null);
            }
            else if (type.IsAssignableFrom(a.GetType()))
            {
                PushObject(a);
            }
            else
            {
                throw new InvalidCastException($"Invalid casting {a} to {type}");
            }
            WykonajNastepnaInstrukcje();
        }
    }
}
