using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Basic
{
    public class IntShitftTest
    {
        [SetUp]
        public void Start() { }

        [Test]
        public void Test1()
        {
            var vm = new VirtualMachine();
            var process = new ShiftTestProcess();

            //shl
            Assert.AreEqual(process.ShiftL(0b0010, 2), vm.StartTestExecution<int>("ShiftL", process, 0b0010, 2));
            Assert.AreEqual(process.ShiftL(-0b0010, -2), vm.StartTestExecution<int>("ShiftL", process, -0b0010, -2));

            //shr
            Assert.AreEqual(process.ShiftR(0b1000, 1), vm.StartTestExecution<int>("ShiftR", process, 0b1000, 1));
            Assert.AreEqual(process.ShiftR(-0b1000, -1), vm.StartTestExecution<int>("ShiftR", process, -0b1000, -1));
        }
    }

    public class ShiftTestProcess
    {
        //Shift left
        public int ShiftL(int value, int amount)
        {
            return value << amount;
        }
        public int ShiftR(int value, int amount)
        {
            return value >> amount;
        }
    }
}
