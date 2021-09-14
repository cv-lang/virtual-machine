using Cvl.VirtualMachine.Core;
using Cvl.VirtualMachine.Debugger.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cvl.VirtualMachine.Debugger.Views.Stack
{
    public class CallStackItemMV : ViewModelBase
    {
        public string MemberName { get; set; }
        
    }

    public class CallStackMV : ViewModelBase
    {
        public ObservableCollection<CallStackItemMV> Methods { get; set; } = new ObservableCollection<CallStackItemMV>();

        internal void Load(ThreadOfControl thread)
        {
            Methods.Clear();

            foreach (var item in thread.CallStack.StosSerializowany)
            {
                var vm = new CallStackItemMV();
                vm.MemberName = $"{item.NazwaTypu}.{item.NazwaMetody}";
                //item.LocalArguments

                Methods.Add(vm);
            }
        }
    }
}
