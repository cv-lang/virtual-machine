using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cvl.VirtualMachine.Core.Serializers;

namespace Cvl.VirtualMachine.Core
{
    /// <summary>
    /// maszyna stanów obsługi wyjątków
    /// </summary>
    public class ExceptionHandlingStateMachine : IDeserializedObject
    {
        
        public ExceptionHandlingStateMachine()
        {
        }

        public void AfterDeserialization()
        {
            Thread = VirtualMachine.Thread;
        }

        public VirtualMachine VirtualMachine { get; set; }

        public ThreadOfControl Thread { get; set; }

        private StanObslugiWyjatkow stan;

        /// <summary>
        /// jeśli rózny od null to jestem w obsłudze wyjątków
        /// </summary>
        public Exception ThrowedException { get; set; }

        private bool IsInExceptionHandling;
        public void SetHandledException(Exception exception)
        {
            IsInExceptionHandling = true;
            ThrowedException = exception;
        }

        public void ClearHandledException()
        {
            IsInExceptionHandling = false;
            ThrowedException = null;
        }

        internal void ExceptionHandlinFromCall(Exception throwedException)
        {
            SetHandledException(throwedException);
            ObslugaRzuconegoWyjatku(throwedException);
        }

        internal void FromLeave(int nextOffset)
        {
            switch(stan)
            {
                case StanObslugiWyjatkow.RzuconoWyjatekIGoObslugujeWTejMetodzie:
                    //czyli zakończyliśmy obsługe wyjątku
                    ClearHandledException();
                    stan = StanObslugiWyjatkow.NormalneWykonanie;
                    //sprawdzam czy nie mam jakiś finally bloków
                    
                    Thread.AktualnaMetoda.TryCatchStack.PopTryBlock(); //w catchu zdejmuje blok ze stosu
                                                                       //jestem w catchu, na jego końcu
                    
                    //miejsce docelowe, do którego skaczemy
                    var instructionIndex = Thread.AktualnaMetoda.PobierzNumerInstrukcjiZOffsetem(nextOffset);
                    Thread.AktualnaMetoda.NumerWykonywanejInstrukcji.SetCurrentInstructionIndex(instructionIndex);

                    //przechodzę po blokach try..catch..finally
                    while (Thread.AktualnaMetoda.TryCatchStack.IsEmptyTryBlock() == false)
                    {
                        var blok = Thread.AktualnaMetoda.TryCatchStack.PeekTryBlock();
                        if (blok.MethodFullName != Thread.AktualnaMetoda.MethodFullName)
                        {
                            break;
                        }

                        if (blok.ExceptionHandlingClause.TryOffset + blok.ExceptionHandlingClause.TryLength < nextOffset)
                        {
                            //przeskakujem ten blok, wrzucam na stos
                            var handlerIndex = Thread.AktualnaMetoda.PobierzNumerInstrukcjiZOffsetem(blok.ExceptionHandlingClause.HandlerOffset);
                            Thread.AktualnaMetoda.NumerWykonywanejInstrukcji.PushExecutionPoint(handlerIndex);
                        }
                        else
                        {
                            //skok nie przeskakuje tego bloku - nie wychodzi z niego
                            //tu kończymy 
                            break;
                        }

                        Thread.AktualnaMetoda.TryCatchStack.PopTryBlock();
                    }
                    break;

                case StanObslugiWyjatkow.NormalneWykonanie:
                    //jestem w leave podczas normalnego wykonania (nie obsługuję wyjątku)
                    //zapamiętuje tylko bloki finally
                    //przechodzę po blokach try..catch..finally

                    //ustawiam docelowy skok do wykonywanej instrukcji
                    var nextInstIndex = Thread.AktualnaMetoda.PobierzNumerInstrukcjiZOffsetem(nextOffset);
                    Thread.AktualnaMetoda.NumerWykonywanejInstrukcji.SetCurrentInstructionIndex(nextInstIndex);
                    
                    //sprawdzam czy podrodze tego skoku nie ma mjakiś finally
                    while (Thread.AktualnaMetoda.TryCatchStack.IsEmptyTryBlock() == false)
                    {
                        var blok = Thread.AktualnaMetoda.TryCatchStack.PeekTryBlock();

                        if (blok.ExceptionHandlingClause.Flags == System.Reflection.ExceptionHandlingClauseOptions.Finally)
                        {
                            if (blok.ExceptionHandlingClause.TryOffset + blok.ExceptionHandlingClause.TryLength < nextOffset)
                            {
                                //przeskakujem ten blok, wrzucam na stos
                                var handlerIndex = Thread.AktualnaMetoda.PobierzNumerInstrukcjiZOffsetem(blok.ExceptionHandlingClause.HandlerOffset);
                                Thread.AktualnaMetoda.NumerWykonywanejInstrukcji.PushExecutionPoint(handlerIndex);
                            }
                            else
                            {
                                //skok nie przeskakuje tego bloku - nie wychodzi z niego
                                //tu kończymy 
                                break;
                            }
                        }

                        Thread.AktualnaMetoda.TryCatchStack.PopTryBlock();
                    }
                    break;
            }
        }
        

        internal void FromEndFinally()
        {
            switch (stan)
            {
                case StanObslugiWyjatkow.NormalneWykonanie:
                    Thread.AktualnaMetoda.NumerWykonywanejInstrukcji.Pop();
                    break;
                case StanObslugiWyjatkow.RzuconoWyjatekIGoObslugujeWTejMetodzie:
                    //jestem w trakcie obsługi wyjątku, zakończyłem blok finally, przechodzę do następnego bloku
                    Thread.AktualnaMetoda.NumerWykonywanejInstrukcji.Pop();                    
                    break;
                case StanObslugiWyjatkow.RzuconoWyjatekAleNiemaObslugiWTejMetodzie:
                    //sprawdzam czy mam jeszcze jakiś bloki do przejscia
                    if(Thread.AktualnaMetoda.NumerWykonywanejInstrukcji.ExecutionPointsStack.Count > 1)
                    {
                        //mam jeszcze bloki do przejscia
                        Thread.AktualnaMetoda.NumerWykonywanejInstrukcji.Pop();
                        Thread.AktualnaMetoda.WykonajNastepnaInstrukcje();
                    } else
                    {
                        //to jest ostatni blok, wychodzę poza metodę
                        Thread.CallStack.PobierzNastepnaMetodeZeStosu();
                        ObslugaRzuconegoWyjatku(ThrowedException);
                    }
                    break;
            }

        }

        internal void FromRethrow()
        {
            //jestem w bloku catch (tylko tam może być rethrow),
            //zdejmuje ze stronu obecny blok - bo już go obsłużyłem - rethrow to ostatnia instrukcja
            Thread.AktualnaMetoda.TryCatchStack.PopTryBlock();

            //włączam ponowną obsługe wyjątku
            ObslugaRzuconegoWyjatku(ThrowedException);
        }

        public void ObslugaRzuconegoWyjatku(object rzuconyWyjatek)
        {
            VirtualMachine.EventHandleException("Przechodzenie przez stos po metodach obsługi wyjątu");

            var thread = VirtualMachine.Thread;
            var aktywnaMetod = VirtualMachine.Thread.AktualnaMetoda;
            var punktyWykonaniaObslugiWyjatku = new Stack<int>();


            var blocks = aktywnaMetod.PobierzBlokiObslugiWyjatkow();
            //czy mam obsługę wyjątków w metodzie
            if (blocks.Any())
            {
                while (true)
                {
                    //czy mam jakąś obsługę w punkcie w którym jestem
                    if(thread.AktualnaMetoda.TryCatchStack.TryCatchBlocks.Any()) 
                    {
                        //mam blok do obsłużenia

                        //pobieram blok
                        var block = thread.AktualnaMetoda.TryCatchStack.PeekTryBlock();

                        VirtualMachine.EventHandleException($"Metoda {aktywnaMetod.NazwaMetody} ... blok obsługi");

                        //obsługujemy pierwszys blok
                        //wirtualnaMaszyna.HardwareContext.PushAktualnaMetode(aktywnaMetod);

                        if (block.ExceptionHandlingClause.Flags == System.Reflection.ExceptionHandlingClauseOptions.Finally)
                        {
                            //obsługa finally
                            //wracam do zwykłej obsługi kodu                            
                            thread.Status = VirtualMachineState.Executing;
                            VirtualMachine.EventHandleException($"Metoda {aktywnaMetod.NazwaMetody}, ... wracam do zwykłej obsługi kodu");

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
                            thread.Status = VirtualMachineState.Executing;
                            VirtualMachine.EventHandleException($"Metoda {aktywnaMetod.NazwaMetody}, ... wracam do zwykłej obsługi kodu");
                            var catchHandlerIndex = aktywnaMetod.PobierzNumerInstrukcjiZOffsetem(block.ExceptionHandlingClause.HandlerOffset);
                            punktyWykonaniaObslugiWyjatku.Push(catchHandlerIndex);

                            stan = StanObslugiWyjatkow.RzuconoWyjatekIGoObslugujeWTejMetodzie;

                            break;
                        }
                        else
                        {
                            VirtualMachine.EventHandleException($"Metoda {aktywnaMetod.NazwaMetody}, ... Brak obsługi blogu Catch w instrukcji Throw");

                            throw new Exception("Brak obsługi blogu Catch w instrukcji Throw");
                        }

                        //return;
                    }
                else
                    {
                        //nie mamy bloków try..catch do obsługi

                        //jestem w trakcie wykonywania wyjatku
                        //sprawdzam czy mam jeszcze jakieś metody na stosie, które mogły by go obsłuzyć
                        if (thread.CallStack.StosSerializowany.Count <= 1)
                        {
                            //nie mamy już metody do obsługi tego wyjątku, wyrzucam go poza VW
                            //throw new Exception("Exception from code in VirtualMachine", (Exception)thread.ThrowedException);
                            
                            //czyszcze stos metod
                            thread.CallStack.Pop();

                            throw new ExceptionRzuconyWyjatekNieObsluzonyWWirtualnejMaszynie(thread.ExceptionHandling.ThrowedException);
                        }
                        else
                        {
                            //mamy jeszcze metody na stosie w których może być obsługa,
                            //najpierw wykonuje wszystie finally

                            stan = StanObslugiWyjatkow.RzuconoWyjatekAleNiemaObslugiWTejMetodzie;
                            break;


                            
                        }
                    }
                }

                //przepisuje wykonywane finally handelry
                aktywnaMetod.NumerWykonywanejInstrukcji.SetCurrentInstructionIndex(punktyWykonaniaObslugiWyjatku.Pop());
                while (punktyWykonaniaObslugiWyjatku.Any())
                {
                    aktywnaMetod.NumerWykonywanejInstrukcji.PushExecutionPoint(punktyWykonaniaObslugiWyjatku.Pop());
                }

            } else
            {
                //metoda niema obsługi błedów
                if (thread.CallStack.IsEmpty())
                {
                    throw new ExceptionRzuconyWyjatekNieObsluzonyWWirtualnejMaszynie(thread.ExceptionHandling.ThrowedException);
                }

                switch (stan)
                {
                    case StanObslugiWyjatkow.NormalneWykonanie:
                        //mamy wyjątek w metodzie bez obsługi
                        thread.CallStack.PobierzNastepnaMetodeZeStosu();

                        //metoda niema obsługi błedów
                        if (thread.CallStack.IsEmpty())
                        {
                            throw new ExceptionRzuconyWyjatekNieObsluzonyWWirtualnejMaszynie(thread.ExceptionHandling.ThrowedException);
                        }

                        ObslugaRzuconegoWyjatku(rzuconyWyjatek);
                        break;
                    case StanObslugiWyjatkow.RzuconoWyjatekAleNiemaObslugiWTejMetodzie:
                        //próbuję pobrać następny element ze stosu
                        //sprawdzam w następnej metodzie czy 
                        thread.CallStack.PobierzNastepnaMetodeZeStosu();

                        
                        break;
                }
            }

            
        }
    }

    public enum StanObslugiWyjatkow
    {
        NormalneWykonanie,
        RzuconoWyjatekIGoObslugujeWTejMetodzie,
        RzuconoWyjatekAleNiemaObslugiWTejMetodzie
    }

    public class ExceptionRzuconyWyjatekNieObsluzonyWWirtualnejMaszynie : Exception
    {
        public ExceptionRzuconyWyjatekNieObsluzonyWWirtualnejMaszynie(Exception throwedException) : base("Wyjątek z kodu w VM",throwedException)
        { }
    }
}
