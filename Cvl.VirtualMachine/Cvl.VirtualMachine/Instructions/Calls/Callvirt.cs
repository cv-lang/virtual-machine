using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Calls
{
    /// <summary>
    /// Calls a late-bound method on an object, pushing the return value onto the evaluation stack.
    /// https://www.ecma-international.org/wp-content/uploads/ECMA-335_6th_edition_june_2012.pdf page 422
    /// </summary>
    public class Callvirt : Call
    {
    }
}
