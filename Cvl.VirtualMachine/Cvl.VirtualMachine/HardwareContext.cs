using Cvl.VirtualMachine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine
{
    public class HardwareContext
    {
        public Stack<ElementBase> Stos { get; set; } = new Stack<ElementBase>();
    }
}
