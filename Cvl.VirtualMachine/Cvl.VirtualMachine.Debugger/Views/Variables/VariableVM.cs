using Cvl.VirtualMachine.Core.Variables.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cvl.VirtualMachine.Debugger.Views.Variables
{
    public class VariableVM
    {
        public int Index { get; set; }
        public string ValueString { get; set; }
        public object Value { get; set; }

        public string TypeName { get; set; }
        public string TypeFullName { get; set; }

        internal void SetVariable(object item)
        {
            var ob = item;
            if (item is ObjectWraper objectWraper)
            {
                ob = objectWraper.Warosc;
                this.ValueString = ob?.ToString();
                this.Value = item;
                this.TypeFullName = $"ObjectWraper<{ob?.GetType().FullName}>";
                this.TypeName = $"OW<{ob?.GetType().Name}>";
            }
            else
            {

                this.ValueString = item?.ToString();
                this.Value = item;
                this.TypeFullName = item?.GetType().FullName;
                this.TypeName = item?.GetType().Name;
            }
        }
    }
}
