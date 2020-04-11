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
                    return CreateInstruction<Call>(instrukcja, i=> i.WirtualnaMaszyna = WirtualnaMaszyna);
                case "callvirt":
                    return CreateInstruction<Callvirt>(instrukcja);
                case "ret":
                    return CreateInstruction<Ret>(instrukcja);
                case "constrained.":
                    return CreateInstruction<Constrained>(instrukcja);
            }
            return null;
        }
    }
}
