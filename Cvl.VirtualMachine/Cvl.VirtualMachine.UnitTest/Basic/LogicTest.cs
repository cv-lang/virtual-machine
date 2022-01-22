using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Basic
{
    public class LogicTest
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void Test1()
        {
            var vm = new VirtualMachine();
            var process = new LogicTestProcess();

            //AND
            Assert.AreEqual(process.And(true, false), vm.StartTestExecution<bool>("And", process, true, false));
            //OR
            Assert.AreEqual(process.Or(true, false), vm.StartTestExecution<bool>("Or", process, true, false));
            //XOR
            Assert.AreEqual(process.Xor(true, false), vm.StartTestExecution<bool>("Xor", process, true, false));
            //NEG
            Assert.AreEqual(process.Neg(true), vm.StartTestExecution<bool>("Neg", process, true));

        }
    }

    internal class LogicTestProcess
    {
        public bool And(bool a, bool b)
        {
            return a & b;
        }
        public bool Or(bool a, bool b)
        {
            return a | b;
        }
        public bool Xor(bool a, bool b)
        {
            return a ^ b;
        }
        public bool Neg(bool a)
        {
            return !a;
        }
    }
}
