using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cvl.VirtualMachine.Core.Attributes;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Basic
{
    public class AsyncTest
    {
        [Test]
        public void Test1()
        {
            var vm = new VirtualMachine();
            var process = new ProcessWithAsync();

            Assert.AreEqual(process.SynchronicStart(), vm.StartTestExecution<string>("SynchronicStart", process));


        }
    }

    public class ProcessWithAsync
    {
        public string SynchronicStart()
        {
            var ret = GetStringAsync().Result;
            return ret;
        }

        [Interpret]
        public async Task<string> GetStringAsync()
        {
            await Task.Delay(1000);
            return "stringTestowy";
        }
    }
}
