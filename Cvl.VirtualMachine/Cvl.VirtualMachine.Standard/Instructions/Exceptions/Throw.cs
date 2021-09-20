using Cvl.VirtualMachine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Exceptions
{
    public class Throw : InstructionBase
    {
        public override void Wykonaj()
        {
            HardwareContext.Status = VirtualMachineState.Exception;
            var rzuconyWyjatek = PopObject();
            MethodContext.WirtualnaMaszyna.EventThrowException(rzuconyWyjatek as Exception);

            ObslugaRzuconegoWyjatku(MethodContext.WirtualnaMaszyna, rzuconyWyjatek);
        }

        public static void ObslugaRzuconegoWyjatku(VirtualMachine wirtualnaMaszyna, object rzuconyWyjatek)
        {
            wirtualnaMaszyna.EventHandleException("Przechodzenie przez stos po metodach obsługi wyjątu");

            var thread = wirtualnaMaszyna.Thread;
            var aktywnaMetod = wirtualnaMaszyna.Thread.AktualnaMetoda;
            var punktyWykonaniaObslugiWyjatku = new Stack<int>();

            while (true)
            {

                var blocks = aktywnaMetod.PobierzBlokiObslugiWyjatkow();

                if (blocks.Any())
                {
                    var block = thread.AktualnaMetoda.TryCatchStack.PeekTryBlock();

                    wirtualnaMaszyna.EventHandleException($"Metoda {aktywnaMetod.NazwaMetody} ... blok obsługi");

                    //obsługujemy pierwszys blok
                    //wirtualnaMaszyna.HardwareContext.PushAktualnaMetode(aktywnaMetod);
                    

                    if (block.ExceptionHandlingClause.Flags == System.Reflection.ExceptionHandlingClauseOptions.Finally)
                    {
                        //obsługa finally
                        //wracam do zwykłej obsługi kodu                            
                        wirtualnaMaszyna.Thread.Status = VirtualMachineState.Executing;
                        wirtualnaMaszyna.EventHandleException($"Metoda {aktywnaMetod.NazwaMetody}, ... wracam do zwykłej obsługi kodu");

                        var finallyHandlerIndex = aktywnaMetod.PobierzNumerInstrukcjiZOffsetem(block.ExceptionHandlingClause.HandlerOffset);
                        punktyWykonaniaObslugiWyjatku.Push(finallyHandlerIndex);
                        thread.AktualnaMetoda.TryCatchStack.PopTryBlock();
                        //wirtualnaMaszyna.Thread.AktualnaMetoda.NumerWykonywanejInstrukcji.SetCurrentInstructionIndex(finallyHandlerIndex);

                    }
                    else if (block.ExceptionHandlingClause.Flags == System.Reflection.ExceptionHandlingClauseOptions.Clause)
                    {
                        //obsluga catch
                        thread.PushException(rzuconyWyjatek);
                        //wracam do zwykłej obsługi kodu                            
                        wirtualnaMaszyna.Thread.Status = VirtualMachineState.Executing;
                        wirtualnaMaszyna.EventHandleException($"Metoda {aktywnaMetod.NazwaMetody}, ... wracam do zwykłej obsługi kodu");
                        var catchHandlerIndex = aktywnaMetod.PobierzNumerInstrukcjiZOffsetem(block.ExceptionHandlingClause.HandlerOffset);
                        punktyWykonaniaObslugiWyjatku.Push(catchHandlerIndex);

                        break;
                    }
                    else
                    {
                        wirtualnaMaszyna.EventHandleException($"Metoda {aktywnaMetod.NazwaMetody}, ... Brak obsługi blogu Catch w instrukcji Throw");

                        throw new Exception("Brak obsługi blogu Catch w instrukcji Throw");
                    }

                    //return;
                }
                else
                {
                    //nie mamy bloków try..catch do obsługi
                    //sprawdzam czy wyjątek nie powinien wyjsc poza VW
                    if(thread.ThrowedException != null)
                    {
                        //jestem w trakcie wykonywania wyjatku
                        //sprawdzam czy mam jeszcze jakieś metody na stosie, które mogły by go obsłuzyć
                        if(thread.CallStack.StosSerializowany.Count <=1)
                        {
                            //nie mamy już metody do obsługi tego wyjątku, wyrzucam go poza VW
                            
                            //czyszcze stos metod
                            thread.CallStack.Pop();

                            throw new ExceptionRzuconyWyjatekNieObsluzonyWWirtualnejMaszynie(thread.ExceptionHandling.ThrowedException);

                            return;
                        }
                    }


                    wirtualnaMaszyna.Thread.CallStack.PobierzNastepnaMetodeZeStosu();
                    aktywnaMetod = wirtualnaMaszyna.Thread.AktualnaMetoda;
                    if (aktywnaMetod == null)
                    {
                        //mamy koniec stosu
                        throw new Exception("Koniec stosu w obsłudze wyjątku", rzuconyWyjatek as Exception);
                    }
                }

            }

            //przepisuje wykonywane finally handelry
            punktyWykonaniaObslugiWyjatku= new Stack<int>( punktyWykonaniaObslugiWyjatku.Reverse());
            thread.AktualnaMetoda.NumerWykonywanejInstrukcji.SetCurrentInstructionIndex(punktyWykonaniaObslugiWyjatku.Pop());

            while(punktyWykonaniaObslugiWyjatku.Any())
            {
                thread.AktualnaMetoda.NumerWykonywanejInstrukcji.PushExecutionPoint(punktyWykonaniaObslugiWyjatku.Pop());
            }


            //while (true)
            //{

            //    var czyObslugujeWyjatek = aktywnaMetod.CzyObslugujeWyjatki();
            //    wirtualnaMaszyna.EventHandleException($"Metoda {aktywnaMetod.NazwaMetody}, czy obsługuje wyjątek {czyObslugujeWyjatek}");
            //    if (czyObslugujeWyjatek)
            //    {

            //        var bloki = aktywnaMetod.PobierzBlokiObslugiWyjatkow();
            //        var blok = bloki.FirstOrDefault();
            //        if (blok != null)
            //        {
            //            wirtualnaMaszyna.EventHandleException($"Metoda {aktywnaMetod.NazwaMetody} ... blok obsługi");

            //            //obsługujemy pierwszys blok
            //            //wirtualnaMaszyna.HardwareContext.PushAktualnaMetode(aktywnaMetod);
            //            wirtualnaMaszyna.Thread.AktualnaMetoda.NumerWykonywanejInstrukcji
            //                = aktywnaMetod.PobierzNumerInstrukcjiZOffsetem(blok.HandlerOffset);

            //            wirtualnaMaszyna.Thread.PushException(rzuconyWyjatek);
            //            if(blok.Flags == System.Reflection.ExceptionHandlingClauseOptions.Finally)
            //            {
            //                //wracam do zwykłej obsługi kodu                            
            //                wirtualnaMaszyna.Thread.Status = VirtualMachineState.Executing;
            //                wirtualnaMaszyna.EventHandleException($"Metoda {aktywnaMetod.NazwaMetody}, ... wracam do zwykłej obsługi kodu");

            //            } else if(blok.Flags == System.Reflection.ExceptionHandlingClauseOptions.Clause)
            //            {
            //                //wracam do zwykłej obsługi kodu                            
            //                wirtualnaMaszyna.Thread.Status = VirtualMachineState.Executing;
            //                wirtualnaMaszyna.EventHandleException($"Metoda {aktywnaMetod.NazwaMetody}, ... wracam do zwykłej obsługi kodu");

            //            }
            //            else
            //            {
            //                wirtualnaMaszyna.EventHandleException($"Metoda {aktywnaMetod.NazwaMetody}, ... Brak obsługi blogu Catch w instrukcji Throw");

            //                throw new Exception("Brak obsługi blogu Catch w instrukcji Throw");
            //            }

            //            return;
            //        }
            //    }

            //    wirtualnaMaszyna.Thread.CallStack.PobierzNastepnaMetodeZeStosu();
            //    aktywnaMetod = wirtualnaMaszyna.Thread.AktualnaMetoda;
            //    if (aktywnaMetod == null)
            //    {
            //        //mamy koniec stosu
            //        throw new Exception("Koniec stosu w obsłudze wyjątku", rzuconyWyjatek as Exception);
            //    }
            //}
        }
    }
}
