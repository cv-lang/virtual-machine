using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Basic.Arithmetic
{
    //UniteTest and TestProcess

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

            //uint
            Assert.AreEqual(process.AddU(1, 2), vm.StartTestExecution<int>("AddU", process, 1, 2));
            Assert.AreEqual(process.SubU(3, 2), vm.StartTestExecution<int>("SubU", process, 3, 2));
            Assert.AreEqual(process.MulU(1, 2), vm.StartTestExecution<int>("MulU", process, 1, 2));
            Assert.AreEqual(process.DivU(4, 2), vm.StartTestExecution<int>("DivU", process, 4, 2));
            Assert.AreEqual(process.ModU(5, 2), vm.StartTestExecution<int>("ModU", process, 5, 2));

            //int
            Assert.AreEqual(process.Add(1, 2), vm.StartTestExecution<int>("Add", process, 1, 2));
            Assert.AreEqual(process.Sub(1, 2), vm.StartTestExecution<int>("Sub", process, 1, 2));
            Assert.AreEqual(process.Mul(1, 2), vm.StartTestExecution<int>("Mul", process, 1, 2));
            Assert.AreEqual(process.Div(4, 2), vm.StartTestExecution<int>("Div", process, 4, 2));
            Assert.AreEqual(process.Add(1, -2), vm.StartTestExecution<int>("Add", process, 1, -2));
            Assert.AreEqual(process.Sub(1, -2), vm.StartTestExecution<int>("Sub", process, 1, -2));
            Assert.AreEqual(process.Mul(1, -2), vm.StartTestExecution<int>("Mul", process, 1, -2));
            Assert.AreEqual(process.Div(4, -2), vm.StartTestExecution<int>("Div", process, 4, -2));

            //decimal
            Assert.AreEqual(process.AddM(1, 2), vm.StartTestExecution<decimal>("AddM", process, 1m, 2m));
            Assert.AreEqual(process.SubM(1, 2), vm.StartTestExecution<decimal>("SubM", process, 1m, 2m));
            Assert.AreEqual(process.MulM(1, 2), vm.StartTestExecution<decimal>("MulM", process, 1m, 2m));
            Assert.AreEqual(process.DivM(4, 2), vm.StartTestExecution<decimal>("DivM", process, 4m, 2m));
            Assert.AreEqual(process.AddM(1, -2), vm.StartTestExecution<decimal>("AddM", process, 1m, -2m));
            Assert.AreEqual(process.SubM(1, -2), vm.StartTestExecution<decimal>("SubM", process, 1m, -2m));
            Assert.AreEqual(process.MulM(1, -2), vm.StartTestExecution<decimal>("MulM", process, 1m, -2m));
            Assert.AreEqual(process.DivM(4, -2), vm.StartTestExecution<decimal>("DivM", process, 4m, -2m));

            //float
            Assert.AreEqual(process.AddF(1, 2), vm.StartTestExecution<float>("AddF", process, 1f, 2f));
            Assert.AreEqual(process.SubF(1, 2), vm.StartTestExecution<float>("SubF", process, 1f, 2f));
            Assert.AreEqual(process.MulF(1, 2), vm.StartTestExecution<float>("MulF", process, 1f, 2f));
            Assert.AreEqual(process.DivF(4, 2), vm.StartTestExecution<float>("DivF", process, 4f, 2f));
            Assert.AreEqual(process.AddF(1, -2.56f), vm.StartTestExecution<float>("AddF", process, 1f, -2.56f));
            Assert.AreEqual(process.SubF(1, -2.1f), vm.StartTestExecution<float>("SubF", process, 1f, -2.1f));
            Assert.AreEqual(process.MulF(1, -2.07f), vm.StartTestExecution<float>("MulF", process, 1f, -2.07f));
            Assert.AreEqual(process.DivF(4, -2.05f), vm.StartTestExecution<float>("DivF", process, 4f, -2.05f));

            //double
            Assert.AreEqual(process.AddD(1, 2), vm.StartTestExecution<double>("AddD", process, 1d, 2d));
            Assert.AreEqual(process.SubD(1, 2), vm.StartTestExecution<double>("SubD", process, 1d, 2d));
            Assert.AreEqual(process.MulD(1, 2), vm.StartTestExecution<double>("MulD", process, 1d, 2d));
            Assert.AreEqual(process.DivD(4, 2), vm.StartTestExecution<double>("DivD", process, 4d, 2d));
            Assert.AreEqual(process.AddD(1, -2.56d), vm.StartTestExecution<double>("AddD", process, 1d, -2.56d));
            Assert.AreEqual(process.SubD(1, -2.1d), vm.StartTestExecution<double>("SubD", process, 1d, -2.1d));
            Assert.AreEqual(process.MulD(1, -2.07d), vm.StartTestExecution<double>("MulD", process, 1d, -2.07d));
            Assert.AreEqual(process.DivD(4, -2.05d), vm.StartTestExecution<double>("DivD", process, 4d, -2.05d));

            //Assert.Pass();
        }
        [Test]
        public void OverflowTest1()
        {
            var vm = new VirtualMachine();
            var process = new ArithmeticTestProcess();

            //intMax
            Assert.Throws<ArgumentOutOfRangeException>(() => process.Add(int.MaxValue, 2147483647));
            Assert.Throws<ArgumentOutOfRangeException>(() => process.Sub(2147483647, 2147483647));
            Assert.Throws<ArgumentOutOfRangeException>(() => process.Mul(2147483647, 2147483647));
            Assert.Throws<ArgumentOutOfRangeException>(() => process.Div(2147483647, 2147483647));
            Assert.Throws<ArgumentOutOfRangeException>(() => process.Add(2147483647, -2147483647));
            Assert.Throws<ArgumentOutOfRangeException>(() => process.Sub(2147483647, -2147483647));
            Assert.Throws<ArgumentOutOfRangeException>(() => process.Mul(2147483647, -2147483647));
            Assert.Throws<ArgumentOutOfRangeException>(() => process.Div(2147483647, -2147483647));
            Assert.AreEqual(process.Add(2147483647, 2147483647), vm.StartTestExecution<int>("Add", process, 2147483647, 2147483647));
            Assert.AreEqual(process.Sub(2147483647, 2147483647), vm.StartTestExecution<int>("Sub", process, 2147483647, 2147483647));
            Assert.AreEqual(process.Mul(2147483647, 2147483647), vm.StartTestExecution<int>("Mul", process, 2147483647, 2147483647));
            Assert.AreEqual(process.Div(2147483647, 2147483647), vm.StartTestExecution<int>("Div", process, 2147483647, 2147483647));
            Assert.AreEqual(process.Add(2147483647, -2147483647), vm.StartTestExecution<int>("Add", process, 2147483647, -2147483647));
            Assert.AreEqual(process.Sub(2147483647, -2147483647), vm.StartTestExecution<int>("Sub", process, 2147483647, -2147483647));
            Assert.AreEqual(process.Mul(2147483647, -2147483647), vm.StartTestExecution<int>("Mul", process, 2147483647, -2147483647));
            Assert.AreEqual(process.Div(2147483647, -2147483647), vm.StartTestExecution<int>("Div", process, 2147483647, -2147483647));
        }

        [Test]
        public void Test2()
        {
            var vm = new VirtualMachine();
            var process = new ArithmeticTestProcess();

            //int
            Assert.AreEqual(process.Mod(4, 2), vm.StartTestExecution<int>("Mod", process, 4, 2));
            //decimal
            Assert.AreEqual(process.Mod(4, 2), vm.StartTestExecution<decimal>("ModM", process, 4m, 2m));
            //float
            Assert.AreEqual(process.Mod(4, 2), vm.StartTestExecution<float>("ModF", process, 4f, 2f));
            //double
            Assert.AreEqual(process.Mod(4, 2), vm.StartTestExecution<double>("ModD", process, 4d, 2d));
        }
    }

    public class ArithmeticTestProcess
    {
        #region simple operation
        #region uint - add, sub, mul, div, mod

        public uint AddU(uint a, uint b)
        {
            return a + b;
        }

        public uint SubU(uint a, uint b)
        {
            return a - b;
        }

        public uint MulU(uint a, uint b)
        {
            return a * b;
        }

        public uint DivU(uint a, uint b)
        {
            return a / b;
        }
        public uint ModU(uint a, uint b)
        {
            return a % b;
        }
        #endregion
        #region int - add, sub, mul, div, mod

        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Sub(int a, int b)
        {
            return a - b;
        }

        public int Mul(int a, int b)
        {
            return a * b;
        }

        public int Div(int a, int b)
        {
            return a / b;
        }
        public int Mod(int a, int b)
        {
            return a % b;
        }
        #endregion
        #region decimal - add, sub, mul, div, mod

        public decimal AddM(decimal a, decimal b)
        {
            return a + b;
        }

        public decimal SubM(decimal a, decimal b)
        {
            return a - b;
        }

        public decimal MulM(decimal a, decimal b)
        {
            return a * b;
        }

        public decimal DivM(decimal a, decimal b)
        {
            return a / b;
        }

        public decimal ModM(decimal a, decimal b)
        {
            return a % b;
        }
        #endregion
        #region float - add, sub, mul, div, mod

        public float AddF(float a, float b)
        {
            return a + b;
        }

        public float SubF(float a, float b)
        {
            return a - b;
        }

        public float MulF(float a, float b)
        {
            return a * b;
        }

        public float DivF(float a, float b)
        {
            return a / b;
        }

        public float ModF(float a, float b)
        {
            return a % b;
        }
        #endregion
        #region double - add, sub, mul, div, mod

        public double AddD(double a, double b)
        {
            return a + b;
        }

        public double SubD(double a, double b)
        {
            return a - b;
        }

        public double MulD(double a, double b)
        {
            return a * b;
        }

        public double DivD(double a, double b)
        {
            return a / b;
        }

        public double ModD(double a, double b)
        {
            return a % b;
        }
        #endregion
        #endregion
    }
}
