using Cvl.ApplicationServer.Logs.Factory;
using Cvl.VirtualMachine.Core.Tools;
using Cvl.VirtualMachine.Test;
using Cvl.VirtualMachine.UnitTest.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cvl.VirtualMachine.TestAppFramework
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            
            

            var factoryLogger= new ApplicationServer.Logs.Factory.LoggerFactory(new ApplicationServer.Logs.Storage.FileLogStorage(), "test-vm");

            var p = new ConvertTestProcess();

            var vm = new Cvl.VirtualMachine.VirtualMachine();
            vm.ActionToExecute(() =>
            {
                p.Convert_i1(200);
            });

            using (vm.Logger = factoryLogger.GetLogger())
            {

                vm.LogMonitor = new ConsoleLogMonitor();
                vm.InterpreteFullNameTypes = "Cvl.VirtualMachine.Test";


                var app = new Application();
                var debuggerVM = new Debugger.Views.Debugger.DebuggerVM();
                debuggerVM.VirtualMachine = vm;
                var window = new Cvl.VirtualMachine.Debugger.MainWindow();
                window.DataContext = debuggerVM;
                app.Run(window);



            }
        }
    }
}
