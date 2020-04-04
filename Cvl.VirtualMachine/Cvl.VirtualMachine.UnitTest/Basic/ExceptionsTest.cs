using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.UnitTest.Basic
{
    public class ExceptionsTest
    {
        [Test]
        public void Test1()
        {
            var vm = new VirtualMachine();
            var process = new ExceptionsTestProces();

            vm.Start<int>("Start2", process);

            Assert.AreEqual(process.Start1(), vm.Start<int>("Start1", process));
            Assert.AreEqual(process.Start2(), vm.Start<int>("Start2", process));

        }
    }

    public class ExceptionsTestProces
    {
        public int Start1()
        {
            try
            {
                throw new Exception("test");
            }
            catch (Exception ex)
            {
                return -1;
            }

            return 0;
        }

        public int Start2()
        {
            try
            {
                methodWitchThrowException();
            } catch(Exception ex)
            {
                return -1;
            }

            return 0;
        }

        private void methodWitchThrowException()
        {
            throw new Exception();
        }
    }
}
