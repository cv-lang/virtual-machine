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
}
