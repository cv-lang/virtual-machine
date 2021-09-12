using Cvl.VirtualMachine.Core;
using Cvl.VirtualMachine.Debugger.Views.Variables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cvl.VirtualMachine.Debugger.Views.LocalVariables
{
    
    public class LocalVariablesVM : VariablesVM
    {       
        internal void Load(MethodState aktualnaMetoda)
        {
            Variables.Clear();

            int i = 0;
            foreach (var item in aktualnaMetoda.LocalVariables.Obiekty.Values)
            {
                var vm = new VariableVM();
                vm.SetVariable(item);
                vm.Index = i;

                Variables.Add(vm);

                i++;
            }
        }
    }
}
