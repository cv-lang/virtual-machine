using Cvl.VirtualMachine.Core;
using Cvl.VirtualMachine.Core.Enums;
using Cvl.VirtualMachine.Core.Variables;
using Cvl.VirtualMachine.Core.Variables.Addresses;
using Cvl.VirtualMachine.Instructions;
using Cvl.VirtualMachine.Instructions.Calls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Cvl.VirtualMachine
{
    public class ThreadOfControl
    {
        /// <summary>
        /// Referencja do wirutalnej maszyny, w której jest wątek
        /// </summary>
        public VirtualMachine WirtualnaMaszyna { get; internal set; }

        /// <summary>
        /// Stos wywołań - tu znajdują się wykonywane metody
        /// </summary>
        public CallStack CallStack { get; set; } = new CallStack();

        public ExceptionHandlingStateMachine ExceptionHandling { get; set; }
        /// <summary>
        /// Globalny numer iteracji - licznyk wykonanych instrukcji
        /// </summary>
        public long NumerIteracji { get; set; }



        /// <summary>
        /// Status wirtualnej maszyny
        /// </summary>
        public VirtualMachineState Status { get; set; }

        /// <summary>
        /// Aktualnie wykonywana instrukcja
        /// </summary>
        private InstructionBase aktualnaInstrukcja;

        /// <summary>
        /// Rzucony wyjątek - przechowywany w trakcie obsługi wyjątków try..catch
        /// </summary>
        public object ThrowedException { get; set; }

        public MethodState AktualnaMetoda => CallStack.PobierzTopMethodState();

        internal void PushAktualnaMetode(MethodState metodaDoWykonania)
        {
            CallStack.Push(metodaDoWykonania);
        }

        //public bool CzyWykonywacInstrukcje { get; set; } = true;
        public Type ConstrainedType { get; internal set; }

       
        public List<object>? HibernateParams { get; set; }

        /// <summary>
        /// Wynik działania wyritalnej maszyny - wątku
        /// </summary>
        
        public object Result { get; internal set; }
               
        public ThreadOfControl(VirtualMachine virtualMachine)
        {
            WirtualnaMaszyna = virtualMachine;
            WirtualnaMaszyna.Thread = this;
            ExceptionHandling = new ExceptionHandlingStateMachine(virtualMachine);            
        }


        /// <summary>
        /// Wykonuje jeden krok/jedną instrukcję
        /// </summary>
        /// <returns></returns>
        public StepExecutionResultEnum Step()
        {
            var zmienneLokalne = AktualnaMetoda.LocalVariables;
            var argumenty = AktualnaMetoda.LocalArguments;
            var stos = CallStack;

            if (WirtualnaMaszyna.BreakpointIterationNumber == NumerIteracji)
            {
                Debugger.Break();
            }
            //try
            {
                //if (NS.Debug.StopIterationNumber == NumerIteracji)
                //{
                //    System.Diagnostics.Debugger.Break();
                //}
                aktualnaInstrukcja = PobierzAktualnaInstrukcje();

                //wrzucam na stron rozpoczete bloki try..catch..finally
                var tryBlocks = AktualnaMetoda.GetBeginTryBlocks();
                AktualnaMetoda.TryCatchStack.PushTryBolcks(tryBlocks, AktualnaMetoda);

                
                //sprawdzam czy instrukcja ma breakpointa
                if(aktualnaInstrukcja.Breakpoint == true)
                {
                    aktualnaInstrukcja.Breakpoint = false;
                    return StepExecutionResultEnum.Breakpoint;
                }


                try
                {
                    //wykonuje instrukcję
                    aktualnaInstrukcja.Wykonaj();
                }
                catch(ExceptionRzuconyWyjatekNieObsluzonyWWirtualnejMaszynie ex)
                {
                    throw (Exception)ex.InnerException;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Błąd w instrukcji {aktualnaInstrukcja} w metodzie {AktualnaMetoda}, iteracja: {NumerIteracji}", ex);
                }


                NumerIteracji++;

                if (CallStack.IsEmpty())
                {                   
                    
                    //kończymy wykonanie
                    return StepExecutionResultEnum.EndExecution;
                }
            }
            //catch (Exception ex)
            //{
            //    CzyWykonywacInstrukcje = false;
            //    //Status = VirtualMachineState.Exception;
            //    //RzuconyWyjatekCalosc = ex.ToString();
            //    //RzuconyWyjatekWiadomosc = ex.Message; ;
            //    //if (Debugger.IsAttached)
            //    //{
            //    //    Debugger.Break();
            //    //}
            //    return;
            //}

            return StepExecutionResultEnum.Continue;
        }

        internal void PushException(object rzuconyWyjatek)
        {
            AktualnaMetoda.EvaluationStack.PushObject(rzuconyWyjatek);
        }

        /// <summary>
        /// wykonuje do następnej instrukcji
        /// jeśli instrukcja to call to całość calla jest debugowana i zatrzymuje się na następnej instrukcji
        /// </summary>
        public void StepOver()
        {
            aktualnaInstrukcja = PobierzAktualnaInstrukcje();

            if(aktualnaInstrukcja is Call call)
            {
                //jeśli call - to przeskakuje go

                var instructions = AktualnaMetoda.Instrukcje;

                //sprawdzam czy nie koniec instrukcji
                if (AktualnaMetoda.NumerWykonywanejInstrukcji.CurrentInstructionIndex + 1 < instructions.Count)
                {
                    //ustawiam breakpointa na nastpno instukcje 
                    instructions[AktualnaMetoda.NumerWykonywanejInstrukcji.CurrentInstructionIndex + 1].Breakpoint = true;
                    //i wykonywanie - z oczekiwanien na breakpointa
                    Execute();
                }
                else
                {
                    //jeśli koniec instrukcji to robię stepa (bo będzie tak ret lub koniec)
                    Step();
                }
            } else
            {
                //jeśli inna instrukcja to robię step
                Step();
            }            
        }

        /// <summary>
        /// Wykonuje całość kodu 
        /// (do momentu breakpoita lub hibernacji)
        /// </summary>
        public void Execute()
        {
            //NS.Debug.VM = this; // do debugowania 

            while (CallStack.IsEmpty()==false && Status != VirtualMachineState.Executed && AktualnaMetoda.CzyWykonywacInstrukcje)
            {
                var result = Step();

                if (result == StepExecutionResultEnum.EndExecution)
                {
                    break;
                } else if(result == StepExecutionResultEnum.Breakpoint)
                {
                    break;
                } 
            }
        }       


        /// <summary>
        /// Wykonuje kod do podanego numeru iteracji
        /// </summary>
        /// <param name="iterationNumber"></param>
        public void ExecuteToIteration(long iterationNumber)
        {
            for (long i = this.NumerIteracji; i < iterationNumber; i++)
            {
                Step();
            }
        }

        public InstructionBase PobierzAktualnaInstrukcje()
        {
            var ai = AktualnaMetoda.Instrukcje[AktualnaMetoda.NumerWykonywanejInstrukcji.CurrentInstructionIndex];
            ai.MethodContext = AktualnaMetoda;
            ai.HardwareContext =  this.WirtualnaMaszyna.Thread;
            return ai;
        }



        //public void WczytajLokalneArgumenty(int iloscArgumentow)
        //{
        //    var lista = new object[iloscArgumentow];
        //    for (int i = iloscArgumentow - 1; i >= 0; i--)
        //    {
        //        var o = Stos.Pop();
        //        lista[i] = o;
        //    }
        //    AktualnaMetoda.LocalArguments.Wczytaj(lista);
        //}

        //public void ZapiszLokalnyArgument(object o, int indeks)
        //{
        //    AktualnaMetoda.LocalArguments.Ustaw(indeks, o);
        //}

        //public object PobierzLokalnyArgument(int indeks)
        //{
        //    var obiekt = AktualnaMetoda.LocalArguments.Pobierz(indeks);
        //    var ow = obiekt as ObjectWraperBase;
        //    if (ow != null)
        //    {
        //        return ow.GetValue();
        //    }
        //    return obiekt;
        //}

        //public void ZapiszLokalnaZmienna(object o, int indeks)
        //{
        //    AktualnaMetoda.LocalVariables.Ustaw(indeks, o);
        //}

        //public object PobierzLokalnaZmienna(int indeks)
        //{
        //    var obiekt = AktualnaMetoda.LocalVariables.Pobierz(indeks);
        //    var ow = obiekt as ObjectWraperBase;
        //    if (ow != null)
        //    {
        //        return ow.GetValue();
        //    }
        //    return obiekt;
        //}

        //public LocalVariableAddress PobierzAdresZmiennejLokalnej(int indeks)
        //{
        //    var adres = new LocalVariableAddress();
        //    adres.Indeks = indeks;
        //    adres.LokalneZmienne = AktualnaMetoda.LocalVariables;

        //    return adres;
        //}

        //public ArgumentAddress PobierzAdresArgumentu(int indeks)
        //{
        //    var adres = new ArgumentAddress();
        //    adres.Indeks = indeks;
        //    adres.LokalneArgumenty = AktualnaMetoda.LocalArguments;

        //    return adres;
        //}

        //public void WykonajNastepnaInstrukcje()
        //{
        //    var am = AktualnaMetoda;
        //    am.NumerWykonywanejInstrukcji++;
        //    am.OffsetWykonywanejInstrukcji
        //        = am.Instrukcje[am.NumerWykonywanejInstrukcji].Instruction.Offset;
        //}

        //public void WykonajSkok(int nowyOffset)
        //{
        //    var am = AktualnaMetoda;
        //    var ins = am.Instrukcje.FirstOrDefault(i => i.Instruction.Offset == nowyOffset);
        //    am.NumerWykonywanejInstrukcji = am.Instrukcje.IndexOf(ins);
        //}

        public void Push(MethodState o)
        {
            CallStack.Push(o);
        }

        //public void Push(ElementBase o)
        //{
        //    Stos.Push(o);
        //}

        /// <summary>
        /// Zwraca obiekt
        /// jeśli jest adres na stosie to zamienia na obiekt
        /// </summary>
        /// <returns></returns>
        public MethodState PopMethodState()
        {
            var ob = CallStack.Pop();
            return ob;
        }

        //public object Pop()
        //{
        //    var ob = Stos.Pop();

        //    return ob;
        //}

        //public object PobierzElementZeStosu(int numerElementuOdSzczytu)
        //{
        //    return Stos.PobierzElementZeStosu(numerElementuOdSzczytu);
        //}
    }
}
