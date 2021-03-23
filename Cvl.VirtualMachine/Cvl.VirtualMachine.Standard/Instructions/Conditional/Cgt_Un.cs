using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Conditional
{
    /// <summary>
    /// Compares two unsigned or unordered values. 
    /// If the first value is greater than the second, the integer value 1 (int32) is pushed onto the evaluation stack; 
    /// otherwise 0 (int32) is pushed onto the evaluation stack.
    /// </summary>
    public class Cgt_Un : InstructionBase
    {
        public override void Wykonaj()
        {
            dynamic b = PopObject();
            dynamic a = PopObject();

            if(a is null)
            {
                a = 0;
            }
            if(b is null)
            {
                b = 0;
            }

            if (b is int  && a is Enum )
            {
                var wynik = (int)a > b ? 1 : 0;
                PushObject(wynik);
            }
            else
            if (b is int || a is int)
            {
                dynamic wynik = a > b ? 1 : 0;
                PushObject(wynik);
            }
            else if (b is double || a is double)
            {
                dynamic wynik = a > b ? 1 : 0;
                PushObject(wynik);
            }
            else if (b is float || a is float)
            {
                dynamic wynik = a > b ? 1 : 0;
                PushObject(wynik);
            }
            else
            {
                //mamy jakiś obiekt więc sprawdzamy czy jest różny
                dynamic wynik = a != b ? 1 : 0;
                PushObject(wynik);
            }

            WykonajNastepnaInstrukcje();
        }
    }
}