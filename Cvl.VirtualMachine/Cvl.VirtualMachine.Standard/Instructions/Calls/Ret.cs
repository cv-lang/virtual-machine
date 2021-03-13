using Cvl.VirtualMachine.Core;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Calls
{
    public class Ret : InstructionBase
    {        
        public override void Wykonaj()
        {
            //mamy wynik
            object wynik = "null";

            //sprawdzam czy jest coś jeszcze na stosie, jeśli jet to jest to wynik, który trzeba zwrócić
            if (MethodContext.EvaluationStack.IsEmpty())
            {
                EventRet();
                //mamy koniec wykonywania procedury (bez wyniku) 
                //MethodContext.CzyWykonywacInstrukcje = false;
                //WirtualnaMaszyna.Status = VirtualMachineState.Executed;
               // return;
            }
            else
            {
                // mamy wynik metody, pobieram ze stosu
                wynik = PopObject();

                //loguje wykonanie ret
                EventRet(wynik);
            }


            //mamy inne metody na stosie wywołań - przekazuje ostatniej wynik
            //ściągam ze stosu obecną metodę
            HardwareContext.PopMethodState();

            //sprawdzam czy koniec wykonania VM - jest coś jeszcze na stosie wywołań - jeśli nie to kończymy wątek i wirtualną maszynę
            if (HardwareContext.CallStack.IsEmpty())
            {
                //mamy koniec wykonywania funkcji (zwracającej wynik)
                MethodContext.CzyWykonywacInstrukcje = false;
                HardwareContext.Status = VirtualMachineState.Executed;
                HardwareContext.Result = wynik; //zwracam wynik na stosie
                return;
            }

            var metodaDoWznowienia = HardwareContext.AktualnaMetoda;
            if (wynik == "null")
            {
                //metoda bez wyniku
            }
            else
            {
                metodaDoWznowienia.PushObject(wynik);
            }

            metodaDoWznowienia.NumerWykonywanejInstrukcji++;

        }
    }
}
