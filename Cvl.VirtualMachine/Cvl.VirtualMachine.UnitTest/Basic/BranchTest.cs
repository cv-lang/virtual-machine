using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Basic
{
    public class BranchTest
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void Test1()
        {
            var vm = new VirtualMachine();
            Cvl.VirtualMachine.Test.VirtualMachineDebug.VirtualMachine = vm;
            var process = new BranchTestProcess();


            //Bge
            Assert.AreEqual(process.Bge(2, 1), vm.StartTestExecution<int>("Bge", process, 2, 1));

            //Beq
            Assert.AreEqual(process.Beq(2, 1), vm.StartTestExecution<int>("Beq", process, 2, 1));

            //Bgt
            Assert.AreEqual(process.Bgt(2, 1), vm.StartTestExecution<int>("Bgt", process, 2, 1));

            //Ble
            Assert.AreEqual(process.Ble(2, 1), vm.StartTestExecution<int>("Ble", process, 2, 1));

            //Blt
            Assert.AreEqual(process.Blt(2, 1), vm.StartTestExecution<int>("Blt", process, 2, 1));

            //Bne
            Assert.AreEqual(process.Bne(2, 1), vm.StartTestExecution<int>("Bne", process, 2, 1));


            //Br
            Assert.AreEqual(process.Br(), vm.StartTestExecution<int>("Br", process));

            //Brfalse
            Assert.AreEqual(process.Brfalse(2), vm.StartTestExecution<int>("Brfalse", process, 0));

            //Brinst
            Assert.AreEqual(process.Brinst(null), vm.StartTestExecution<int>("Brinst", process, null));

            //Brnull
            Assert.AreEqual(process.Brnull(null), vm.StartTestExecution<int>("Brnull", process, null));

            //Brinst
            Assert.AreEqual(process.Brinst(2), vm.StartTestExecution<int>("Brinst", process, 2));

            //Brtrue
            Assert.AreEqual(process.Brtrue(2), vm.StartTestExecution<int>("Brtrue", process, 2));
        }

        [Test]
        public void Test2()
        {
            var vm = new VirtualMachine();
            Cvl.VirtualMachine.Test.VirtualMachineDebug.VirtualMachine = vm;
            var process = new BranchTestProcess();

            object i = new Int32();

            ///Isinst_intSimple
            ///

            var t = process.Isinst_intSimple(0);

            Assert.AreEqual(process.Isinst_intSimple(0), vm.StartTestExecution<bool>("Isinst_intSimple", process, 0));

            //Assert.AreEqual(process.Isinst_int(0), vm.StartTestExecution<int>("Isinst_int", process, 0));            
        }

        [Test]
        public void Test3()
        {
            var vm = new VirtualMachine();
            Cvl.VirtualMachine.Test.VirtualMachineDebug.VirtualMachine = vm;
            var process = new BranchTestProcess();

            

            //Brzero
            Assert.AreEqual(process.Brzero(0), vm.StartTestExecution<int>("Brzero", process, 0));

            //Ceq
            Assert.AreEqual(process.Ceq(2, 2), vm.StartTestExecution<int>("Ceq", process, 2, 2));

            //Cgt
            Assert.AreEqual(process.Cgt(2, 1), vm.StartTestExecution<int>("Cgt", process, 2, 1));

            //Cgt_Un
            Assert.AreEqual(process.Cgt_Un(2, 2), vm.StartTestExecution<int>("Cgt_Un", process, 2, 2));

            //Clt
            Assert.AreEqual(process.Clt(2, 2), vm.StartTestExecution<int>("Clt", process, 2, 2));
        }
    }

    public class BranchTestProcess
    {
        #region conditions
        public int Bge(int value1, int value2)
        {
            if (value1 > value2)
                return 1;
            return 0;
        }

        public int Beq(int value1, int value2)
        {
            if (value1 == value2)
                return 1;
            return 0;
        }

        public int Bgt(uint value1, uint value2)
        {
            if (value1 > value2)
                return 1;
            return 0;
        }

        public int Ble(int value1, int value2)
        {
            if (value1 <= value2)
                return 1;
            return 0;
        }

        public int Blt(int value1, int value2)
        {
            if (value1 < value2)
                return 1;
            return 0;
        }

        public int Bne(uint value1, uint value2)
        {
            if (value1 != value2)
                return 1;
            return 0;
        }

        #endregion
        #region branches

        public int Br()
        {
            return 1;
        }

        public int Brfalse(object value)
        {
            if (value == null)
                return 1;
            else if (value is bool b && b == false)
                return 1;
            else if (value is int i && i == 0)
                return 1;
            return 0;
        }

        public int Brinst(object value)
        {
            if (value != null)
                return 0;
            return 1;
        }

        public int Brnull(object value)
        {
            if (value == null)
                return 1;
            return 0;
        }

        public int Brtrue(object value)
        {
            if (value != null)
                return 1;
            else if (value is bool istrue && istrue  == true)
                return 2;
            else if (value is int i && i != 0)
                return 3;
            return 0;
        }

        public bool Isinst_intSimple(object value)
        {
            return value is int;
        }

        public int Isinst_int(object value)
        {
            if (value is int)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }


        public int Brzero(object value)
        {
            if (value is int i)
            {
                if (i == 0)
                {
                    return 1;
                }
            }
            return 0;
        }

        #endregion
        #region integerconditions
        public int Ceq(dynamic value1, dynamic value2)
        {
            if (value1 == value2)
                return 1;
            return 0;
        }

        public int Cgt(dynamic value1, dynamic value2)
        {
            if (value1 > value2)
                return 1;
            return 0;
        }

        public int Cgt_Un(uint value1, uint value2)
        {
            if (value1 > value2)
                return 1;
            return 0;
        }

        public int Clt(dynamic value1, dynamic value2)
        {
            if (value1 < value2)
                return 1;
            return 0;
        }

        #endregion
    }
}
