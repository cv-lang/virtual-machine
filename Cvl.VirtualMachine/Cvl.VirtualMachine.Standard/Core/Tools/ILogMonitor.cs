using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Cvl.VirtualMachine.Core.Tools
{
    public interface ILogMonitor
    {
        void EventRet(object ret, long iterationNumber);
        void EventCall(MethodBase method, List<object> parameters, int callLevel, long iterationNumber);
    }

    public class ConsoleLogMonitor : ILogMonitor
    {
        public void EventCall(MethodBase method, List<object> parameters, int callLevel, long iterationNumber)
        {
            for (int i = 0; i < callLevel; i++)
            {
                Console.Write(" ");
            }
            Console.WriteLine($"{method.Name}({string.Join(",", parameters)})");
        }

        public void EventRet(object ret, long iterationNumber)
        {            
        }
    }
}
