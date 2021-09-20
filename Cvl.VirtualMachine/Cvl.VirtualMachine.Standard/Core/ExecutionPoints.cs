using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Core
{
    public class ExecutionPoint
    {
        /// <summary>
        /// numer instrukcji która jest wywoływana
        /// </summary>
        public int ExecutionInstructionIndex { get; set; }
    }

    public class ExecutionPoints
    {
        public Stack<ExecutionPoint> ExecutionPointsStack { get; set; } = new Stack<ExecutionPoint>();
        public int CurrentInstructionIndex => ExecutionPointsStack.Peek().ExecutionInstructionIndex;

        public ExecutionPoints()
        {
            ExecutionPointsStack.Push(new ExecutionPoint());
        }

        internal void Pop()
        {
            ExecutionPointsStack.Pop();
        }

        public static ExecutionPoints operator ++(ExecutionPoints c1)
        {
            c1.ExecutionPointsStack.Peek().ExecutionInstructionIndex++;
            return c1;
        }

        internal void SetCurrentInstructionIndex(int v)
        {
            ExecutionPointsStack.Peek().ExecutionInstructionIndex = v;
        }

        internal void PushExecutionPoint(int handlerIndex)
        {
            ExecutionPointsStack.Push(new ExecutionPoint() { ExecutionInstructionIndex = handlerIndex });
        }
    }
}
