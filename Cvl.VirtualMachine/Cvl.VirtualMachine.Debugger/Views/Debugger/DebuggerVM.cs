using Cvl.VirtualMachine.Debugger.Base;
using Cvl.VirtualMachine.Debugger.Views.EvaluationStack;
using Cvl.VirtualMachine.Debugger.Views.Instructions;
using Cvl.VirtualMachine.Debugger.Views.LocalArguments;
using Cvl.VirtualMachine.Debugger.Views.LocalVariables;
using Cvl.VirtualMachine.Debugger.Views.Stack;
using Cvl.VirtualMachine.Debugger.Views.TryCatchBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cvl.VirtualMachine.Debugger.Views.Debugger
{
    public class DebuggerVM : ViewModelBase
    {
        public VirtualMachine VirtualMachine { get; set; }
        public InstructionsVM Instructions { get; set; } = new InstructionsVM();
        public CallStackMV CallStack { get; set; } = new CallStackMV();
        public LocalArgumentsVM LocalArguments { get; set; } = new LocalArgumentsVM();
        public LocalVariablesVM LocalVariables { get; set; } = new LocalVariablesVM();
        public EvaluationStackVM EvaluationStack { get; set; } = new EvaluationStackVM();
        public TryCatchBlocksVM TryCatchBlocks { get; set; } = new TryCatchBlocksVM();

        private long currentIteration;
        public long CurrentIteration
        {
            get => currentIteration;
            set
            {
                if (value != currentIteration)
                {
                    currentIteration = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        //RunToIterationNumber
        private long runToIterationNumber;
        public long RunToIterationNumber
        {
            get => runToIterationNumber;
            set
            {
                if (value != runToIterationNumber)
                {
                    runToIterationNumber = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        internal void Load()
        {
            if(VirtualMachine.Thread.Status == Core.VirtualMachineState.Executed)
            {
                return;
            }

            CallStack.Load(VirtualMachine.Thread);
            Instructions.Load(VirtualMachine.Thread.AktualnaMetoda);
            LocalArguments.Load(VirtualMachine.Thread.AktualnaMetoda);
            LocalVariables.Load(VirtualMachine.Thread.AktualnaMetoda);
            EvaluationStack.Load(VirtualMachine.Thread.AktualnaMetoda);
            TryCatchBlocks.Load(VirtualMachine.Thread);
        }

        

        internal void Refresh()
        {
            Load();
            CurrentIteration = VirtualMachine.Thread.NumerIteracji;
        }

        internal void Execute()
        {
            VirtualMachine.Execute();
            CurrentIteration = VirtualMachine.Thread.NumerIteracji;
        }

        internal void Step()
        {
            VirtualMachine.Step();
            CurrentIteration = VirtualMachine.Thread.NumerIteracji;
            Refresh();
        }

        internal void StepOver()
        {
            VirtualMachine.StepOver();
            CurrentIteration = VirtualMachine.Thread.NumerIteracji;
            Refresh();
        }

        internal void ExecuteToIteration()
        {
            VirtualMachine.ExecuteToIteration(RunToIterationNumber);
            CurrentIteration = VirtualMachine.Thread.NumerIteracji;
        }

        internal void StepToCursor()
        {
            if(Instructions.SelectedInstruction != null)
            {
                Instructions.SelectedInstruction.SetBreakpoint();
                VirtualMachine.Execute();
                CurrentIteration = VirtualMachine.Thread.NumerIteracji;
            }
            
        }
    }
}
