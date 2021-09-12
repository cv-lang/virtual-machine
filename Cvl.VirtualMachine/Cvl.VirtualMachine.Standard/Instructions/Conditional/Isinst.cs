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
            dynamic b = PopObject();
            if (b != null)
            {
                var typ = b.GetType();
                var typOperanda = (Type)Instruction.Operand;
                if (typOperanda.IsAssignableFrom(typ))
                {
                    //mamy ten sam typ


                    if (typOperanda.IsByRef == false)
                    {
                        //musimy zrobić sztuczny boxing - 
                        //Push(new BoxWraper() { Warosc = b });
                        Push(new BoxWraper() { Warosc = true });
                    }
                    else
                    {
                        //wrzucamy normalnya obiekt
                        PushObject(b);
                    }


                    //wrzucamy normalnya obiekt
                    //PushObject(b);
                }
                else
                {
                    PushObject(null);
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