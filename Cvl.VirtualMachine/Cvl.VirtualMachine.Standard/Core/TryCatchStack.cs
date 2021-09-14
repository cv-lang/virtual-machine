using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Cvl.VirtualMachine.Core
{
    public class TryCatchBlock
    {
        public ExceptionHandlingClause ExceptionHandlingClause { get; internal set; }
        public string MethodFullName { get; internal set; }
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
                b.ExceptionHandlingClause = item;
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
