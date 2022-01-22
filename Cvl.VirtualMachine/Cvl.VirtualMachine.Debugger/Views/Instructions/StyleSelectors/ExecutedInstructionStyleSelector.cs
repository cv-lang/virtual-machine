using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Cvl.VirtualMachine.Debugger.Views.Instructions.StyleSelectors
{
    public class ExecutedInstructionStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is InstructionVM inst)
            {
                if (inst.IsExecuted)
                {
                    return IsExecutedStyle;
                }
                else
                {
                    return NormalStyle;
                }
            }
            return null;
        }
        public Style IsExecutedStyle { get; set; }
        public Style NormalStyle { get; set; }
    }
}
