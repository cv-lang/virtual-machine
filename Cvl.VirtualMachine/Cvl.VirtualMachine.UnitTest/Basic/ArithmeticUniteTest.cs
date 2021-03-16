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

            //int
            Assert.AreEqual(process.Add(1, 2), vm.StartTestExecution<int>("Add", process, 1, 2));
            Assert.AreEqual(process.Sub(1, 2), vm.StartTestExecution<int>("Sub", process, 1, 2));
            Assert.AreEqual(process.Mul(1, 2), vm.StartTestExecution<int>("Mul", process, 1, 2));
            Assert.AreEqual(process.Div(4, 2), vm.StartTestExecution<int>("Div", process, 4, 2));

            //decimal
            Assert.AreEqual(process.AddM(1, 2), vm.StartTestExecution<decimal>("AddM", process, 1m, 2m));
            Assert.AreEqual(process.SubM(1, 2), vm.StartTestExecution<decimal>("SubM", process, 1m, 2m));
            Assert.AreEqual(process.MulM(1, 2), vm.StartTestExecution<decimal>("MulM", process, 1m, 2m));
            Assert.AreEqual(process.DivM(4, 2), vm.StartTestExecution<decimal>("DivM", process, 4m, 2m));

            //float
            Assert.AreEqual(process.AddF(1, 2), vm.StartTestExecution<float>("AddF", process, 1f, 2f));
            Assert.AreEqual(process.SubF(1, 2), vm.StartTestExecution<float>("SubF", process, 1f, 2f));
            Assert.AreEqual(process.MulF(1, 2), vm.StartTestExecution<float>("MulF", process, 1f, 2f));
            Assert.AreEqual(process.DivF(4, 2), vm.StartTestExecution<float>("DivF", process, 4f, 2f));

            //double
            Assert.AreEqual(process.AddD(1, 2), vm.StartTestExecution<double>("AddD", process, 1d, 2d));
            Assert.AreEqual(process.SubD(1, 2), vm.StartTestExecution<double>("SubD", process, 1d, 2d));
            Assert.AreEqual(process.MulD(1, 2), vm.StartTestExecution<double>("MulD", process, 1d, 2d));
            Assert.AreEqual(process.DivD(4, 2), vm.StartTestExecution<double>("DivD", process, 4d, 2d));

            //Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            var vm = new VirtualMachine();
            var process = new ArithmeticTestProcess();

            //int
            Assert.AreEqual(process.Mod(4, 2), vm.StartTestExecution<int>("Mod", process, 4, 2));
            //decimal
            Assert.AreEqual(process.Mod(4, 2), vm.StartTestExecution<decimal>("Mod", process, 4m, 2m));
            //float
            Assert.AreEqual(process.Mod(4, 2), vm.StartTestExecution<float>("Mod", process, 4f, 2f));
            //double
            Assert.AreEqual(process.Mod(4, 2), vm.StartTestExecution<double>("Mod", process, 4d, 2d));
        }
    }

    public class ArithmeticTestProcess
    {
        #region simple operation 
        #region int - add, sub, mul, div

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
        #region decimal - add, sub, mul, div

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
        #region float - add, sub, mul, div

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
        #region double - add, sub, mul, div

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
