using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cvl.VirtualMachine.Debugger.Views.Debugger
{
    /// <summary>
    /// Logika interakcji dla klasy DebuggerControl.xaml
    /// </summary>
    public partial class DebuggerControl : UserControl
    {
        public DebuggerVM ViewModel => DataContext as DebuggerVM;

        public DebuggerControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Load();
        }
        

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.F10
                && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                ViewModel.StepToCursor();
                ViewModel.Refresh();
                e.Handled = true;
            } else if (e.SystemKey == Key.F10)
            {
                ViewModel.StepOver();
                ViewModel.Refresh();
                e.Handled = true;
            } else if (e.Key == Key.F11)
            {
                ViewModel.Step();
                ViewModel.Refresh();
                e.Handled = true;
            } 

        }

        

        private void btnStep_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Step();
            ViewModel.Refresh();
        }

        private void btnStepOver_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StepOver();
            ViewModel.Refresh();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Execute();
            ViewModel.Refresh();
        }

        private void btnExecuteToSelected_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StepToCursor();
            ViewModel.Refresh();
        }
    }
}
