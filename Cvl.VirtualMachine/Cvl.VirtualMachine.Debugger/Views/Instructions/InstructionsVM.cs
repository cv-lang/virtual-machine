using Cvl.VirtualMachine.Debugger.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cvl.VirtualMachine.Debugger.Views.Instructions
{
    public class InstructionVM
    {
        public bool IsExecuted { get; set; } = false;
        public string Offset { get; set; }
        public string Code { get; set; }
        public string Operand { get; set; }
        public Cvl.VirtualMachine.Instructions.InstructionBase Instruction { get; internal set; }

        internal void SetBreakpoint()
        {
            Instruction.Breakpoint = true;
        }
    }

    public class InstructionsVM : ViewModelBase
    {
        public ObservableCollection<InstructionVM> Instructions { get; set; } = new ObservableCollection<InstructionVM>();

        private InstructionVM selectedInstruction;
        public InstructionVM SelectedInstruction
        {
            get => selectedInstruction;
            set
            {
                if (value != selectedInstruction)
                {
                    selectedInstruction = value;
                    base.RaisePropertyChanged();
                }
            }
        }


        internal void Load(Core.MethodState aktualnaMetoda)
        {
            Instructions.Clear();

            var instructions = aktualnaMetoda.Instrukcje;

            int i = 0;
            foreach (var item in instructions)
            {
                var vm = new InstructionVM();
                vm.Offset = string.Format("IL_{0:X4}:", item.Offset);
                vm.Code = item.Code;
                vm.Operand = item.Operand;
                vm.Instruction = item;
                if(i == aktualnaMetoda.NumerWykonywanejInstrukcji)
                {  vm.IsExecuted = true; }
                else { vm.IsExecuted = false; }

                i++;
                Instructions.Add(vm);
            }
        }
    }
}
