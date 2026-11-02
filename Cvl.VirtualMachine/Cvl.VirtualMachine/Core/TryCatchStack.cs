using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Cvl.VirtualMachine.Core
{
    public class TryCatchBlock
    {
        /// <summary>
        /// Klauzula należy do metody, więc w stanie zapisujemy tylko jej numer
        /// </summary>
        public MethodState Metoda { get; internal set; }
        public int NumerKlauzuli { get; internal set; }
        public ExceptionHandlingClause ExceptionHandlingClause => Metoda.Klauzule[NumerKlauzuli];
        public string MethodFullName { get; internal set; }

        /// <summary>
        /// Wyjątek obsługiwany w catch tego bloku - potrzebny dla rethrow
        /// </summary>
        public Exception Zlapany { get; internal set; }
    }

    public class TryCatchStack
    {
        public Stack<TryCatchBlock> TryCatchBlocks { get; set; } = new Stack<TryCatchBlock>();

        internal void PushTryBolcks(List<ExceptionHandlingClause> tryBlocks, MethodState aktualnaMetoda)
        {
            var methodDesc = aktualnaMetoda.PobierzOpisMetody();
            var methodFullName = $"{methodDesc.DeclaringType.FullName}.{methodDesc.Name}";

            tryBlocks.Reverse(); //liste wrzuce na stron, dla tego musze zamienić kolejność
            foreach (var item in tryBlocks)
            {
                var b = new TryCatchBlock();
                b.Metoda = aktualnaMetoda;
                b.NumerKlauzuli = aktualnaMetoda.Klauzule.IndexOf(item);
                b.MethodFullName = methodFullName;

                TryCatchBlocks.Push(b);
            }
        }

        internal TryCatchBlock PeekTryBlock()
        {
            return TryCatchBlocks.Peek();
        }

        internal bool IsEmptyTryBlock()
        {
            return TryCatchBlocks.Count <= 0;
        }

        internal TryCatchBlock PopTryBlock()
        {
            return TryCatchBlocks.Pop();
        }
    }
}
