using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

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
        [XmlIgnore]
        public Stack<ExecutionPoint> ExecutionPointsStack { get; set; } = new Stack<ExecutionPoint>();
        public int CurrentInstructionIndex => ExecutionPointsStack.Peek().ExecutionInstructionIndex;

        public List<ExecutionPoint> ExecutionPointsStackSerializowany
        {
            get
            {
                return ExecutionPointsStack.ToList();
            }
            set
            {
                var l = value.ToList();
                l.Reverse();
                foreach (var item in l)
                {
                    ExecutionPointsStack.Push(item);
                }
            }
        }

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

        /// <summary>
        /// Wyjątek porzuca zaplanowane punkty (np. cel skoku z leave, gdy poleciał w finally)
        /// </summary>
        internal void Reset(int v)
        {
            while (ExecutionPointsStack.Count > 1)
            {
                ExecutionPointsStack.Pop();
            }
            SetCurrentInstructionIndex(v);
        }

        internal void PushExecutionPoint(int handlerIndex)
        {
            ExecutionPointsStack.Push(new ExecutionPoint() { ExecutionInstructionIndex = handlerIndex });
        }
    }
}
