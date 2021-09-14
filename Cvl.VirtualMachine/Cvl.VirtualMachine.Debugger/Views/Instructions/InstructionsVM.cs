using Cvl.VirtualMachine.Debugger.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cvl.VirtualMachine.Debugger.Views.Instructions
{
    public class InstructionVM : ViewModelBase
    {
        public string OffsetHex { get; set; }
        public long Offset { get; set; }
        public string Code { get; set; }
        public string Operand { get; set; }
        public Cvl.VirtualMachine.Instructions.InstructionBase Instruction { get; internal set; }
                
        private bool isExecuted;
        public bool IsExecuted
        {
            get => isExecuted;
            set
            {
                if (value != isExecuted)
                {
                    isExecuted = value;
                    base.RaisePropertyChanged();
                }
            }
        }

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


        private string actualMethodUniqueId;
        internal void Load(Core.MethodState aktualnaMetoda)
        {
            var mthodDescription = aktualnaMetoda.PobierzOpisMetody();
            var uniqeMethodId = mthodDescription.DeclaringType.FullName + "." + mthodDescription.Name;
            
            if(uniqeMethodId == actualMethodUniqueId)
            {
                //mamy tak sama metode, odswiezamy tylko wskaznik

                int i = 0;
                foreach (var vm in Instructions)
                {
                    vm.IsExecuted = false;
                    if (i == aktualnaMetoda.NumerWykonywanejInstrukcji)
                    { vm.IsExecuted = true; }

                    i++;
                }

            } else
            {
                //mamy inna metode 
                actualMethodUniqueId = uniqeMethodId;
                Instructions.Clear();

                var instructions = aktualnaMetoda.Instrukcje;

                int i = 0;
                foreach (var item in instructions)
                {
                    var vm = new InstructionVM();
                    vm.Offset = item.Offset;
                    vm.OffsetHex = string.Format("IL_{0:X4}:", item.Offset);
                    vm.Code = item.Code;
                    vm.Operand = item.Operand;
                    vm.Instruction = item;
                    if (i == aktualnaMetoda.NumerWykonywanejInstrukcji)
                    { vm.IsExecuted = true; }
                    else { vm.IsExecuted = false; }

                    i++;
                    Instructions.Add(vm);
                }
            }            
        }
    }
}
