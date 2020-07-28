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
            vm.InterpreteFullNameTypes = "Cvl.VirtualMachine.Test";
            vm.Start(() =>
            {
                p.Start();
            });
        }
    }
}
