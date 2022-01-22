using Cvl.VirtualMachine.Debugger.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cvl.VirtualMachine.Debugger.Views.Variables
{
    public class VariablesVM : ViewModelBase
    {
        public ObservableCollection<VariableVM> Variables { get; set; } = new ObservableCollection<VariableVM>();

        private VariableVM selectedVariable;
        public VariableVM SelectedVariable
        {
            get => selectedVariable;
            set
            {
                if( value != selectedVariable)
                {
                    selectedVariable = value;
                    base.RaisePropertyChanged();
                }
            }
        }
    }
}
