using Cvl.VirtualMachine.Core.Tools;
using Cvl.VirtualMachine.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cvl.VirtualMachine.TestAppFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new ProcesTest();

            var vm = new Cvl.VirtualMachine.VirtualMachine();
            vm.LogMonitor = new ConsoleLogMonitor();
            vm.InterpreteFullNameTypes = "Cvl.VirtualMachine.Test";
            vm.Start(() =>
            {
                p.Start();
            });
        }
    }
}
