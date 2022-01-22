using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Basic
{
    public class SwitchTableTest
    {
        [SetUp]
        public void Start() { }

        [Test]
        public void Test1()
        {
            var vm = new VirtualMachine();
            var process = new SwitchTableTestProcess();

            Assert.AreEqual(process.Switch(2, new int[] { 1, 2, 3, 4, 5 }), vm.StartTestExecution<int>("Switch", process, 2, new int[] { 1, 2, 3, 4, 5 }));

        }
    }

    public class SwitchTableTestProcess
    {
        public int Switch(int i, int[] arr)
        {
            return arr[i];
        }
    }
}