using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Basic.Arithmetic
{
    public class ArithmeticUniteTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var vm = new VirtualMachine();
            var process = new ArithmeticTestProcess();

            Assert.AreEqual(process.AddTest(1, 2), vm.Start<int>("AddTest", process, 1, 2));
           
            //Assert.Pass();
        }
    }
}
