using Cvl.VirtualMachine.Test;
using Mono.Reflection;
using System;
using System.Windows;
using Cvl.VirtualMachine.Core.Tools;

namespace Cvl.VirtualMachine.TestApp
{

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {



            //var factoryLogger = new ApplicationServer.Logs.Factory.LoggerFactory(new ApplicationServer.Logs.Storage.FileLogStorage(), "test-vm");

            var p = new ProcesTest();

            var vm = new Cvl.VirtualMachine.VirtualMachine();
            vm.ActionToExecute(() =>
            {
                p.Start();
            });

            //using (vm.Logger = factoryLogger.GetLogger())
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