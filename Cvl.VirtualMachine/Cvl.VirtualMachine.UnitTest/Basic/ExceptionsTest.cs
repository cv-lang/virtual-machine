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
            Cvl.VirtualMachine.Test.VirtualMachineDebug.VirtualMachine = vm;
            var process = new ExceptionsTestProces();

            var ret2 = process.Start1();
            var ret = vm.StartTestExecution<int>("Start1", process);

            Assert.AreEqual(ret2, ret);
        }

        [Test]
        public void Test2()
        {
            var vm = new VirtualMachine();
            var process = new ExceptionsTestProces();

            var ret2 = process.Start2();
            var ret = vm.StartTestExecution<int>("Start2", process);

            Assert.AreEqual(ret2, ret);
        }

        [Test]
        public void Test3()
        {
            var vm = new VirtualMachine();
            Cvl.VirtualMachine.Test.VirtualMachineDebug.VirtualMachine = vm;
            var process = new ExceptionsTestProces();

            var ret2 = process.Start3();
            var ret = vm.StartTestExecution<int>("Start3", process);

            Assert.AreEqual(ret2, ret);
        }

        [Test]
        public void Test4()
        {
            var vm = new VirtualMachine();
            Cvl.VirtualMachine.Test.VirtualMachineDebug.VirtualMachine = vm;
            var process = new ExceptionsTestProces();

            var ret2 = process.Start4();
            var ret = vm.StartTestExecution<int>("Start4", process);

            Assert.AreEqual(ret2, ret);
        }

        [Test]
        public void Test5()
        {
            var vm = new VirtualMachine();
            Cvl.VirtualMachine.Test.VirtualMachineDebug.VirtualMachine = vm;
            var process = new ExceptionsTestProces();

            var ret2 = process.Start5();
            var ret = vm.StartTestExecution<int>("Start5", process);

            Assert.AreEqual(ret2, ret);
        }

        [Test]
        public void Test6()
        {
            var vm = new VirtualMachine();
            Cvl.VirtualMachine.Test.VirtualMachineDebug.VirtualMachine = vm;
            var process = new ExceptionsTestProces();

            var ret2 = process.Start6();
            var ret = vm.StartTestExecution<int>("Start6", process);

            Assert.AreEqual(ret2, ret);
        }

        [Test]
        public void Test7()
        {
            var vm = new VirtualMachine();
            Cvl.VirtualMachine.Test.VirtualMachineDebug.VirtualMachine = vm;
            var process = new ExceptionsTestProces();

            var ret2 = process.Start7();
            var ret = vm.StartTestExecution<int>("Start7", process);

            Assert.AreEqual(ret2, ret);
        }

        [Test]
        public void Test8()
        {
            var vm = new VirtualMachine();
            Cvl.VirtualMachine.Test.VirtualMachineDebug.VirtualMachine = vm;
            var process = new ExceptionsTestProces();

            Assert.Throws<TestException>(() => process.Start8());
            Assert.Throws<TestException>(() => vm.StartTestExecution<int>("Start8", process));

            //Assert.AreEqual(ret2, ret);
        }

        [Test]
        public void Test9()
        {
            var vm = new VirtualMachine();
            Cvl.VirtualMachine.Test.VirtualMachineDebug.VirtualMachine = vm;
            var process = new ExceptionsTestProces();

            var ret2 = process.Start9();
            var ret = vm.StartTestExecution<int>("Start9", process);

            Assert.AreEqual(ret2, ret);
        }

        [Test]
        public void Test10()
        {
            var vm = new VirtualMachine();
            Cvl.VirtualMachine.Test.VirtualMachineDebug.VirtualMachine = vm;
            var process = new ExceptionsTestProces();

            var ret2 = process.Start10();
            var ret = vm.StartTestExecution<int>("Start10", process);

            Assert.AreEqual(ret2, ret);
        }

        [Test]
        public void Test11()
        {
            var vm = new VirtualMachine();
            Cvl.VirtualMachine.Test.VirtualMachineDebug.VirtualMachine = vm;
            var process = new ExceptionsTestProces();

            var ret2 = process.Start11();
            var ret = vm.StartTestExecution<ReferencjaInt>("Start11", process);

            Assert.AreEqual(ret2.Value, ret.Value);
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
                i += 1;
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

        public int Start4()
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

            try
            {
                interpretetMethod1();
            }
            catch (Exception ex)
            {
                i += 2;
            }

            return i;
        }

        public int Start5()
        {
            int i = 0;
            try
            {
                i++;
                try
                {
                    i++;
                    methodWitchThrowException();
                    i++;
                }
                catch (Exception ex1)
                {
                    i++;
                    throw;
                }
                i++;
            }
            catch (Exception ex2)
            {
                i++;
            }
            i++;
            return i;
        }

        public int Start6()
        {
            int i = 0;
            try
            {
                i++;
                try
                {
                    i++;
                    methodWitchThrowException();
                    i++;
                }
                finally
                {
                    i++;
                }
                i++;
            }
            catch (TestException ex2)
            {
                i++;
            }
            i++;
            return i;
        }

        [Interpret]
        public int Start7()
        {
            int i = 0;
            try
            {
                i += 1;
                try
                {
                    i += 2;
                    methodWitchThrowException();
                    i += 3;
                }
                finally
                {
                    i += 4;
                }
                i += 5;
            }
            catch (TestException ex2)
            {
                i += 6;
                i += ex2.SomeValue;
            }
            finally
            {
                i += 7;
            }
            i += 8;
            return i;
        }

        public void Start8()
        {
            int i = 0;
            try
            {
                i += 1;
                try
                {
                    i += 2;
                    methodWitchThrowException();
                    i += 3;
                }
                finally
                {
                    i += 4;
                }
                i += 5;
            }
            catch (TestException ex2)
            {
                throw;
            }
            finally
            {
                i += 7;
            }
            i += 8;
        }

        [Interpret]
        public int Start9()
        {
            using (var t = new DisposableTestObject())
            {

                int i = 0;
                try
                {
                    i += 1;
                    try
                    {
                        i += 2;
                        interpretetMethod3();
                        i += 3;
                    }
                    finally
                    {
                        i += 4;
                    }
                    i += 5;
                }
                catch (TestException ex2)
                {
                }

                i += 8;
                return i;

            }
        }

        [Interpret]
        public int Start10()
        {
            int i = 0;
            try
            {
                i += 2;
                interpretetMethod3();
                i += 3;
            }
            finally
            {
                i += 4;
            }
            i += 5;

            return i;
        }

        [Interpret]
        public ReferencjaInt Start11()
        {
            ReferencjaInt i = new ReferencjaInt(); ;
            try
            {
                i.Value += 1;
                methodWitchThrowException();
                i.Value += 2;
            }
            catch (Exception ex)
            {
                i.Value++;
                return i;
            }
            finally
            {
                i.Value += 4;
            }
            i.Value += 5;



            return i;
        }

        private void methodWitchThrowException()
        {
            throw new TestException() { SomeValue = 3 };
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

        [Interpret]
        private int interpretetMethod3()
        {
            int i = 0;
            try
            {
                try
                {
                    i++;
                    interpretetMethod2();
                }
                catch (Exception ex)
                {
                    i++;
                }

                try
                {
                    interpretetMethod2();
                }
                catch (Exception ex)
                {
                    return i;
                }
            }
            finally
            {
                i++;
            }

            return i;
        }
    }

    public class ReferencjaInt
    {
        public int Value { get; set; }
    }

    public class TestException : Exception
    {
        public int SomeValue { get; set; }
    }

    public class DisposableTestObject : IDisposable
    {
        public int SomeProperty { get; set; }

        public void Dispose()
        {

        }
    }
}
