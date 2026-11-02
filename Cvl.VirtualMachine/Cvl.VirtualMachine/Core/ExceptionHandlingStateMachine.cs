using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Cvl.VirtualMachine.Core.Serializers;

namespace Cvl.VirtualMachine.Core
{
    /// <summary>
    /// Obsługa wyjątków w interpreterze: szukanie catch po typie, kolejność finally, odwijanie stosu metod.
    /// Cały stan leży na ramkach metod (TryCatchStack, ExecutionPoints), więc hibernacja działa także w catch i finally.
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

        /// <summary>
        /// Ostatnio rzucony wyjątek
        /// </summary>
        public Exception ThrowedException { get; set; }

        internal void ExceptionHandlinFromCall(Exception throwedException)
        {
            ObslugaRzuconegoWyjatku(throwedException);
        }

        /// <summary>
        /// Wyjście z try lub catch: bloki, z których wychodzimy, znikają ze stosu, a ich finally wykonują się przed skokiem,
        /// od wewnętrznego do zewnętrznego. Blok, w którym cel skoku nadal leży, zostaje.
        /// </summary>
        internal void FromLeave(int nextOffset)
        {
            var metoda = Thread.AktualnaMetoda;
            metoda.EvaluationStack.PobierzStos().Clear();

            var bloki = metoda.TryCatchStack;
            var doFinally = new List<int>();
            while (bloki.IsEmptyTryBlock() == false)
            {
                var klauzula = bloki.PeekTryBlock().ExceptionHandlingClause;
                var zostajeWTry = nextOffset > klauzula.TryOffset && nextOffset < klauzula.TryOffset + klauzula.TryLength;
                var zostajeWHandlerze = nextOffset >= klauzula.HandlerOffset && nextOffset < klauzula.HandlerOffset + klauzula.HandlerLength;
                if (zostajeWTry || zostajeWHandlerze)
                {
                    break;
                }

                if (klauzula.Flags == ExceptionHandlingClauseOptions.Finally)
                {
                    doFinally.Add(metoda.PobierzNumerInstrukcjiZOffsetem(klauzula.HandlerOffset));
                }

                bloki.PopTryBlock();
            }

            var punkty = metoda.NumerWykonywanejInstrukcji;
            punkty.SetCurrentInstructionIndex(metoda.PobierzNumerInstrukcjiZOffsetem(nextOffset));
            for (int i = doFinally.Count - 1; i >= 0; i--)
            {
                punkty.PushExecutionPoint(doFinally[i]);
            }
        }

        /// <summary>
        /// Koniec finally: wracamy do następnego punktu wykonania, a gdy to było ostatnie finally ramki
        /// w trakcie odwijania - wyjątek idzie dalej do metody wywołującej.
        /// </summary>
        internal void FromEndFinally()
        {
            var metoda = Thread.AktualnaMetoda;
            var punkty = metoda.NumerWykonywanejInstrukcji;

            if (punkty.ExecutionPointsStack.Count > 1)
            {
                punkty.Pop();
                if (punkty.ExecutionPointsStack.Count == 1 && metoda.WyjatekDoCatch != null)
                {
                    WejdzDoCatch(metoda);
                }
                return;
            }

            var wyjatek = metoda.WyjatekWTrakcie;
            metoda.WyjatekWTrakcie = null;
            ZdejmijRamke(wyjatek);
            ObslugaRzuconegoWyjatku(wyjatek);
        }

        /// <summary>
        /// rethrow leży w catch, więc rzucamy wyjątek złapany przez blok, w którego handlerze jesteśmy
        /// </summary>
        internal void FromRethrow()
        {
            var metoda = Thread.AktualnaMetoda;
            var offset = metoda.OffsetWykonywanejInstrukcji;
            var blok = metoda.TryCatchStack.TryCatchBlocks.FirstOrDefault(b =>
                b.Zlapany != null
                && offset >= b.ExceptionHandlingClause.HandlerOffset
                && offset < b.ExceptionHandlingClause.HandlerOffset + b.ExceptionHandlingClause.HandlerLength);

            if (blok == null)
            {
                throw new InvalidOperationException($"rethrow poza blokiem catch w metodzie {metoda.MethodFullName}");
            }

            ObslugaRzuconegoWyjatku(blok.Zlapany);
        }

        /// <summary>
        /// Szuka obsługi od bieżącej ramki w górę stosu metod. Znaleziony catch dostaje wyjątek po wykonaniu finally
        /// z bloków wewnętrznych. Ramka bez pasującego catch wykonuje swoje finally i dopiero potem znika ze stosu.
        /// </summary>
        public void ObslugaRzuconegoWyjatku(Exception wyjatek)
        {
            ThrowedException = wyjatek;

            while (true)
            {
                var metoda = Thread.AktualnaMetoda;
                var offset = metoda.OffsetWykonywanejInstrukcji;
                var bloki = metoda.TryCatchStack;
                var doFinally = new List<int>();
                TryCatchBlock zlapal = null;

                while (bloki.IsEmptyTryBlock() == false)
                {
                    var blok = bloki.PeekTryBlock();
                    var klauzula = blok.ExceptionHandlingClause;

                    //wyjątek rzucony w handlerze tego bloku nie jest już objęty jego try
                    if (offset >= klauzula.TryOffset && offset < klauzula.TryOffset + klauzula.TryLength)
                    {
                        if (klauzula.Flags == ExceptionHandlingClauseOptions.Clause && klauzula.CatchType.IsInstanceOfType(wyjatek))
                        {
                            zlapal = blok;
                            break;
                        }

                        if (klauzula.Flags == ExceptionHandlingClauseOptions.Finally || klauzula.Flags == ExceptionHandlingClauseOptions.Fault)
                        {
                            doFinally.Add(metoda.PobierzNumerInstrukcjiZOffsetem(klauzula.HandlerOffset));
                        }
                    }

                    bloki.PopTryBlock();
                }

                if (zlapal == null && doFinally.Count == 0)
                {
                    ZdejmijRamke(wyjatek);
                    continue;
                }

                metoda.EvaluationStack.PobierzStos().Clear();
                Thread.Status = VirtualMachineState.Executing;
                var punkty = metoda.NumerWykonywanejInstrukcji;

                if (zlapal == null)
                {
                    //samo finally: ostatnie w kolejności leży na dnie, jego koniec oznacza wyjście z ramki
                    metoda.WyjatekWTrakcie = wyjatek;
                    metoda.WyjatekDoCatch = null;
                    punkty.Reset(doFinally[doFinally.Count - 1]);
                    for (int i = doFinally.Count - 2; i >= 0; i--)
                    {
                        punkty.PushExecutionPoint(doFinally[i]);
                    }
                    return;
                }

                zlapal.Zlapany = wyjatek;
                metoda.WyjatekWTrakcie = null;
                metoda.WyjatekDoCatch = wyjatek;
                punkty.Reset(metoda.PobierzNumerInstrukcjiZOffsetem(zlapal.ExceptionHandlingClause.HandlerOffset));
                for (int i = doFinally.Count - 1; i >= 0; i--)
                {
                    punkty.PushExecutionPoint(doFinally[i]);
                }

                if (doFinally.Count == 0)
                {
                    WejdzDoCatch(metoda);
                }
                return;
            }
        }

        private void WejdzDoCatch(MethodState metoda)
        {
            metoda.EvaluationStack.PobierzStos().Clear();
            metoda.PushObject(metoda.WyjatekDoCatch);
            metoda.WyjatekDoCatch = null;
        }

        private void ZdejmijRamke(Exception wyjatek)
        {
            Thread.Status = VirtualMachineState.Exception;
            Thread.CallStack.Pop();
            if (Thread.CallStack.IsEmpty())
            {
                throw new ExceptionRzuconyWyjatekNieObsluzonyWWirtualnejMaszynie(wyjatek);
            }
        }
    }

    public class ExceptionRzuconyWyjatekNieObsluzonyWWirtualnejMaszynie : Exception
    {
        public ExceptionRzuconyWyjatekNieObsluzonyWWirtualnejMaszynie(Exception throwedException) : base("Wyjątek z kodu w VM",throwedException)
        { }
    }
}
