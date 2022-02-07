using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cvl.VirtualMachine.Core.Attributes;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Proces.FromLife
{
    public class StringConstrudtionTest
    {
        [Test]
        public void Test1()
        {
            var proces = new StringConstrudtionTestProcess();
            var vm = new VirtualMachine();
            var vmWynik = vm.StartTestExecution<string>("Start", proces);

            proces = new StringConstrudtionTestProcess();
            var wynik = proces.Start();
            Assert.AreEqual(wynik, vmWynik);
        }
    }

    public class StringConstrudtionTestProcess
    {
        [Interpret]
        public string Start()
{
    int i = 4;
    var text = $"Some text with {i} value";
    return text;
}
    }
}
