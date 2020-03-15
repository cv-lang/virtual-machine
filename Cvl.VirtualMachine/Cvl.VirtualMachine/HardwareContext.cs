using Cvl.VirtualMachine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine
{
    public class HardwareContext
    {
        public Stack Stos { get; set; } = new Stack();
    }
}
