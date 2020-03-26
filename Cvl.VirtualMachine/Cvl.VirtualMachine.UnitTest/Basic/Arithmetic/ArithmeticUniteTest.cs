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

            Assert.AreEqual(process.Add(1, 2), vm.Start<int>("Add", process, 1, 2));
            Assert.AreEqual(process.Sub(1, 2), vm.Start<int>("Sub", process, 1, 2));
            Assert.AreEqual(process.Mul(1, 2), vm.Start<int>("Mul", process, 1, 2));
            Assert.AreEqual(process.Div(4, 2), vm.Start<int>("Div", process, 4, 2));

            //Assert.Pass();
        }
    }
}
