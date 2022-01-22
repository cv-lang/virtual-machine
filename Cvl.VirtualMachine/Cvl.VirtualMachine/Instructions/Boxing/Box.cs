using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Boxing
{
    /// <summary>
    /// Converts a value type to an object reference (type O).
    /// https://msdn.microsoft.com/pl-pl/library/system.reflection.emit.opcodes.box(v=vs.110).aspx 
    /// </summary>
    public class Box : InstructionBase
    {     
        public override void Wykonaj()
        {
            //nic nie robię - box i unbox jest robiony przez środowisko wykonujące
            //nie muszę tego emulować
            WykonajNastepnaInstrukcje();
        }
    }
}
