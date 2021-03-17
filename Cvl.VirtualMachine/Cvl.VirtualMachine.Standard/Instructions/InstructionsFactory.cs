using Cvl.VirtualMachine.Instructions.Base;
using Cvl.VirtualMachine.Instructions.Calls;
using Cvl.VirtualMachine.Instructions.Special;
using Cvl.VirtualMachine.Instructions.Storage;
using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cvl.VirtualMachine.Instructions
{
    public interface IInstructionFactory
    {
        InstructionBase CreateInstruction(Instruction instrukcja);
        VirtualMachine WirtualnaMaszyna { get; set; }
    }

    public class InstructionFactory : IInstructionFactory
    {
        public VirtualMachine WirtualnaMaszyna { get; set; }

        public virtual InstructionBase CreateInstruction(Instruction instrukcja)
        {
            return null;
        }

        public T CreateInstruction<T>(Instruction instruction, Action<T> a = null)
            where T : InstructionBase, new()
        {
            var intst = new T();
            intst.HardwareContext = WirtualnaMaszyna.Thread;
            intst.MethodContext = intst.HardwareContext.AktualnaMetoda;
            intst.Inicialize(instruction);
            if(a != null)
            {
                a.Invoke(intst);
            }
            return intst;
        }

        public T CreateIndexedInstruction<T>(Instruction instruction, int? index = null)
            where T : IndexedInstruction, new()
        {
            var intst = new T();
            intst.Inicialize(instruction, index);
            return intst;
        }
    }

    public  class InstructionsFactory
    {
        public InstructionBase UtworzInstrukcje(Instruction instrukcja, VirtualMachine wirtualnaMaszyna)
        {
            if(instructionFactories == null)
            {
                RegisterInstructionFactory();
            }

            foreach (var facotry in instructionFactories)
            {
                facotry.WirtualnaMaszyna = wirtualnaMaszyna;
                var inst = facotry.CreateInstruction(instrukcja);
                if(inst != null)
                {
                    return inst;
                }
            }

            return null;
        }


        private List<IInstructionFactory> instructionFactories = null;

        public void RegisterInstructionFactory()
        {
            instructionFactories = new List<IInstructionFactory>();
            foreach (Type mytype in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                .Where(mytype => mytype.GetInterfaces().Contains(typeof(IInstructionFactory))))
            {
                instructionFactories.Add((IInstructionFactory)Activator.CreateInstance(mytype));
            }
        }

        
    }
}
