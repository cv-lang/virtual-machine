using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Core
{
    public enum VirtualMachineState
    {
        Stoped,
        Executing,
        Exception,
        Executed,
        Hibernated,
        ExceptionFromVWCore
    }
}
