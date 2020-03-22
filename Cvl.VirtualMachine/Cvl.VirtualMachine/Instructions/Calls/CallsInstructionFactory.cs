using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Calls
{
    public class CallsInstructionFactory : InstructionFactory
    {
        public override InstructionBase CreateInstruction(Instruction instrukcja)
        {
            switch (instrukcja.OpCode.Name)
            {
                case "call":
                    var call= CreateInstruction<Call>(instrukcja);
                    call.WirtualnaMaszyna = WirtualnaMaszyna;
                    return call;
                case "callvirt":
                    return CreateInstruction<Callvirt>(instrukcja);
                case "ret":
                    return CreateInstruction<Ret>(instrukcja);
            }
            return null;
        }
    }
}
