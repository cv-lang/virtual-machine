using Cvl.VirtualMachine.Core.Attributes;
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

            var ret2 = process.Start1();
            var ret = vm.Start<int>("Start1", process);
            
            Assert.AreEqual(ret2, ret);
        }

        [Test]
        public void Test2()
        {
            var vm = new VirtualMachine();
            var process = new ExceptionsTestProces();

            var ret2 = process.Start2();
            var ret = vm.Start<int>("Start2", process);

            Assert.AreEqual(ret2, ret);
        }

        [Test]
        public void Test3()
        {
            var vm = new VirtualMachine();
            var process = new ExceptionsTestProces();

            var ret2 = process.Start3();
            var ret = vm.Start<int>("Start3", process);

            Assert.AreEqual(ret2, ret);
        }
    }

    public class ExceptionsTestProces
    {
        public int Start1()
        {
            int i = 0;
            try
            {
                throw new Exception("test");
            }
            catch (Exception ex)
            {
                i+= 1;
            }
            i += 1;

            return i;
        }


        public int Start2()
        {
            int i = 0;   

            try
            {
                methodWitchThrowException();
            }
            catch (Exception ex)
            {
                i += 1;
            }
            i += 1;

            return i;
        }

        public int Start3()
        {
            int i = 0;     

            try
            {
                interpretetMethod1();
            }
            catch (Exception ex)
            {
                i += 1;
            }
            i += 1;

            return i;
        }

        private void methodWitchThrowException()
        {
            throw new Exception();
        }

        [Interpret]
        private int interpretetMethod1()
        {
            try
            {
                interpretetMethod2();
            }
            catch (Exception ex)
            {

            }

            interpretetMethod2();
            return 1;
        }

        [Interpret]
        private void interpretetMethod2()
        {
            throw new Exception("Testowy wyjątek");
        }
    }
}
