using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Boxing
{
    /// <summary>
    /// Converts the boxed representation of a value type to its unboxed form.
    /// https://msdn.microsoft.com/pl-pl/library/system.reflection.emit.opcodes.unbox(v=vs.110).aspx
    /// </summary>
    public class Unbox : InstructionBase
    {       
        public override void Wykonaj()
        {
            //nic nie robię - box i unbox jest robiony przez środowisko wykonujące
            //nie muszę tego emulować
            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}
