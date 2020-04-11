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
            if (HardwareContext.Stos.IsEmpty())
            {
                //mamy koniec wykonywania procedury (bez wyniku) 
                HardwareContext.CzyWykonywacInstrukcje = false;
                //WirtualnaMaszyna.Status = VirtualMachineState.Executed;
                return;
            }

            //mamy wynik
            var dane = HardwareContext.PopObject();
            if (dane is Metoda)
            {

                //mamy metodę która nie zwraca 
                var metodaDoWznowienia = dane as Metoda;
                HardwareContext.AktualnaMetoda = metodaDoWznowienia;
                HardwareContext.AktualnaMetoda.NumerWykonywanejInstrukcji++;
            }
            else
            {
                //najpierw mamy wynik potem dane metody
                var wynik = dane;
                //sprawdzam czy jest coś jeszcze na stosie
                if (HardwareContext.Stos.IsEmpty())
                {
                    //mamy koniec wykonywania funkcji (zwracającej wynik)
                    HardwareContext.CzyWykonywacInstrukcje = false;
                    HardwareContext.Status = VirtualMachineState.Executed;
                    HardwareContext.PushObject(wynik); //zwracam wynik na stosie
                    return;
                }


                var metodaDoWznowienia = HardwareContext.PopObject() as Metoda;
                HardwareContext.PushObject(wynik); //zwracam wynik na stosie
                HardwareContext.AktualnaMetoda = metodaDoWznowienia;
                HardwareContext.AktualnaMetoda.NumerWykonywanejInstrukcji++;
            }
        }
    }
}
