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
            

            //sprawdzam czy jest coś jeszcze na stosie
            if (MethodContext.Stos.IsEmpty())
            {
                EventRet();
                //mamy koniec wykonywania procedury (bez wyniku) 
                MethodContext.CzyWykonywacInstrukcje = false;
                //WirtualnaMaszyna.Status = VirtualMachineState.Executed;
                return;
            }

            //mamy wynik
            var dane = PopObject();
            

            if (dane is MethodState)
            {

                //mamy metodę która nie zwraca 
                var metodaDoWznowienia = dane as MethodState;
                MethodContext.AktualnaMetoda = metodaDoWznowienia;
                MethodContext.AktualnaMetoda.NumerWykonywanejInstrukcji++;
            }
            else
            {
                //loguje wykonanie ret
                EventRet(dane);

                //najpierw mamy wynik potem dane metody
                var wynik = dane;
                //sprawdzam czy jest coś jeszcze na stosie
                if (MethodContext.Stos.IsEmpty())
                {
                    //mamy koniec wykonywania funkcji (zwracającej wynik)
                    MethodContext.CzyWykonywacInstrukcje = false;
                    MethodContext.Status = VirtualMachineState.Executed;
                    PushObject(wynik); //zwracam wynik na stosie
                    return;
                }


                var metodaDoWznowienia = PopObject() as MethodState;
                PushObject(wynik); //zwracam wynik na stosie
                MethodContext.AktualnaMetoda = metodaDoWznowienia;
                MethodContext.AktualnaMetoda.NumerWykonywanejInstrukcji++;
            }
        }
    }
}
