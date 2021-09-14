using Cvl.VirtualMachine.Debugger.Base;
using Cvl.VirtualMachine.Debugger.Views.Variables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cvl.VirtualMachine.Debugger.Views.TryCatchBlocks
{
    public class TryCatchBlockVM : ViewModelBase
    {
        public int TryOffset { get; set; }
        public int TryLength { get; set; }
        public int HandlerOffset { get; set; }
        public int HandlerLength { get; set; }
        public ExceptionHandlingClauseOptions Flags { get; set; }
        public string MethodFullName { get; set; }
    }

    public class TryCatchBlocksVM : ViewModelBase
    {
        public ObservableCollection<TryCatchBlockVM> TryCatchBlocks { get; set; } = new ObservableCollection<TryCatchBlockVM>();

        internal void Load(ThreadOfControl thread)
        {
            TryCatchBlocks.Clear();

            foreach (var item in thread.TryCatchStack.TryCatchBlocks)
            {
                var vm = new TryCatchBlockVM();
                vm.TryOffset = item.ExceptionHandlingClause.TryOffset;
                vm.TryLength = item.ExceptionHandlingClause.TryLength;
                vm.HandlerOffset = item.ExceptionHandlingClause.HandlerOffset;
                vm.HandlerLength = item.ExceptionHandlingClause.HandlerLength;
                vm.Flags = item.ExceptionHandlingClause.Flags;
                vm.MethodFullName = item.MethodFullName;

                TryCatchBlocks.Add(vm);
            }
        }
    }
}
