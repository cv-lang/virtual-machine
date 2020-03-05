using Cvl.VirtualMachine.Instructions.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Storage
{
    /// <summary>
    /// Loads an argument (referenced by a specified index value) onto the stack.
    /// </summary>
    public class Ldarg : IndexedInstruction
    {
        public override void Wykonaj()
        {
            var o = PobierzLokalnyArgument(Index);
            PushObject(o);
            WykonajNastepnaInstrukcje();
        }

        
    }
}
