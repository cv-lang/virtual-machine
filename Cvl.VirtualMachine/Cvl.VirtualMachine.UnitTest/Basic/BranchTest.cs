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
            Assert.AreEqual(process.Brinst(2), vm.StartTestExecution<int>("Brinst", process, 2));

            //Brnull
            Assert.AreEqual(process.Brnull(null), vm.StartTestExecution<int>("Brnull", process, null));

            //Brtrue
            Assert.AreEqual(process.Brinst(2), vm.StartTestExecution<int>("Brtrue", process, 2));

            //Brzero
            Assert.AreEqual(process.Brinst(0), vm.StartTestExecution<int>("Brzero", process, 0));

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

        public int Brfalse(dynamic value)
        {
            if (value == null)
                return 1;
            else if (value is bool && value == false)
                return 1;
            else if (value == 0)
                return 1;
            return 0;
        }

        public int Brinst(dynamic value)
        {
            if (value != null)
                return 1;
            return 0;
        }

        public int Brnull(dynamic value)
        {
            if (value == null)
                return 1;
            return 0;
        }

        public int Brtrue(dynamic value)
        {
            if (value != null)
                return 1;
            else if (value is bool && value == true)
                return 1;
            else if (value != 0)
                return 1;
            return 0;
        }

        public int Brzero(dynamic value)
        {
            if (value == 0)
                return 1;
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

        //TODO
        public bool Isinst(dynamic value)
        {
            return true;
        }


    }
}
