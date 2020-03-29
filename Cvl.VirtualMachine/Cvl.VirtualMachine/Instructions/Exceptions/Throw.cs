using Cvl.VirtualMachine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Exceptions
{
    public class Throw : InstructionBase
    {
        public override void Wykonaj()
        {
            HardwareContext.Status = VirtualMachineState.Exception;
            var rzuconyWyjatek = HardwareContext.PopObject();
            ObslugaRzuconegoWyjatku(HardwareContext.WirtualnaMaszyna, rzuconyWyjatek);
        }

        public static void ObslugaRzuconegoWyjatku(VirtualMachine wirtualnaMaszyna, object rzuconyWyjatek)
        {
            var aktywnaMetod = wirtualnaMaszyna.HardwareContext.AktualnaMetoda;
            //while (true)
            //{
            //    var czyObslugujeWyjatek = aktywnaMetod.CzyObslugujeWyjatki();
            //    if (czyObslugujeWyjatek)
            //    {
            //        var bloki = aktywnaMetod.PobierzBlokiObslugiWyjatkow();
            //        var blok = bloki.FirstOrDefault();
            //        if (blok != null)
            //        {
            //            //obsługujemy pierwszys blok
            //            wirtualnaMaszyna.AktualnaMetoda = aktywnaMetod;
            //            wirtualnaMaszyna.AktualnaMetoda.NumerWykonywanejInstrukcji
            //                = aktywnaMetod.PobierzNumerInstrukcjiZOffsetem(blok.HandlerStart.Offset);

            //            wirtualnaMaszyna.Stos.PushObject(rzuconyWyjatek); 
            //            if (blok.HandlerType == ExceptionHandlerType.Catch)
            //            {
            //                //wracam do zwykłej obsługi kodu                            
            //                wirtualnaMaszyna.Status = VirtualMachineState.Executing;
            //            }

            //            return;
            //        }
            //    }

            //    aktywnaMetod = wirtualnaMaszyna.Stos.PobierzNastepnaMetodeZeStosu();
            //    if (aktywnaMetod == null)
            //    {
            //        //mamy koniec stosu
            //        throw new Exception("Koniec stosu w obsłudze wyjątku", rzuconyWyjatek as Exception);
            //    }
            //}
        }
    }
}
