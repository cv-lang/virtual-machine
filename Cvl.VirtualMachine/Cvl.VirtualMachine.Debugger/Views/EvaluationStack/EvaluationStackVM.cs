using Cvl.VirtualMachine.Core;
using Cvl.VirtualMachine.Debugger.Views.Variables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cvl.VirtualMachine.Debugger.Views.EvaluationStack
{   

    public class EvaluationStackVM : VariablesVM
    {        
        internal void Load(MethodState aktualnaMetoda)
        {
            Variables.Clear();

            foreach (var item in aktualnaMetoda.EvaluationStack.StosSerializowany)
            {
                var vm = new VariableVM();
                vm.SetVariable(item);

                Variables.Add(vm);
            }
        }
    }
}
