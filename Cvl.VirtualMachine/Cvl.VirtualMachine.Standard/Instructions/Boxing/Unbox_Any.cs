using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Boxing
{
    /// <summary>
    /// Converts the boxed representation of a type specified in the instruction to its unboxed form. 
    /// </summary>
    public class Unbox_Any : InstructionBase
    {       

        public override void Wykonaj()
        {
            //nic nie robię - box i unbox jest robiony przez środowisko wykonujące
            //nie muszę tego emulować
            HardwareContext.WykonajNastepnaInstrukcje();
        }
    }
}
