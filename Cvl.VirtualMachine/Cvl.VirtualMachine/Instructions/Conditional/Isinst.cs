using Cvl.VirtualMachine.Core.Variables.Addresses;
using Cvl.VirtualMachine.Core.Variables.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    /// <summary>
    /// Tests whether an object reference (type O) is an instance of a particular class.
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.isinst?view=netframework-4.8
    /// </summary>
    public class Isinst : InstructionBase
    {        
        public override void Wykonaj()
        {
            var b = PopObject();
            if (b != null && ((Type)Instruction.Operand).IsAssignableFrom(b.GetType()))
            {
                if (CzyPrawda(b))
                {
                    //wynik isinst to sam obiekt - "o as T" i "o is T t" go używają
                    PushObject(b);
                }
                else
                {
                    //zero i false: VM nie odróżnia pudełka od wartości, więc brtrue wziąłby je za "nie jest T".
                    //Zostaje znacznik true - "o is int" działa, "o as int?" z zerem dałby true (znane ograniczenie)
                    Push(new BoxWraper() { Warosc = true });
                }
            }
            else
            {
                PushObject(null);
            }

            WykonajNastepnaInstrukcje();
        }
    }
}