using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Basic.Casting
{
    //UniteTest and TestProcess
    public class CastingUniteTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var vm = new VirtualMachine();
            var process = new CastingTestProcess();

            //int
            Assert.AreEqual(process.CastIB((byte)45), vm.StartTestExecution<object>("CastIB", process,(byte)45)); //TODO: tu jest problem z konvertowaniem byte na int, którego formalnie nie ma!
            Assert.AreEqual(process.CastIF(234.4f), vm.StartTestExecution<int>("CastIF", process, 234.4f));
            Assert.AreEqual(process.CastID(234.4d), vm.StartTestExecution<int>("CastID", process, 234.4d));
            Assert.AreEqual(process.CastIM(234.4m), vm.StartTestExecution<int>("CastIM", process, 234.4m));

            //decimal
            Assert.AreEqual(process.CastMB((byte)45), vm.StartTestExecution<decimal>("CastMB", process, (byte)45));
            Assert.AreEqual(process.CastMF(234.4f), vm.StartTestExecution<decimal>("CastMF", process, 234.4f));
            Assert.AreEqual(process.CastMD(234.4d), vm.StartTestExecution<decimal>("CastMD", process, 234.4d));
            Assert.AreEqual(process.CastMI(45), vm.StartTestExecution<decimal>("CastMI", process, 45));

            //long
            Assert.AreEqual(process.CastLB((byte)45), vm.StartTestExecution<long>("CastLB", process, (byte)45));
            Assert.AreEqual(process.CastLF(234.4f), vm.StartTestExecution<long>("CastLF", process, 234.4f));
            Assert.AreEqual(process.CastLD(234.4d), vm.StartTestExecution<long>("CastLD", process, 234.4d));
            Assert.AreEqual(process.CastLI(45), vm.StartTestExecution<long>("CastLI", process, 45));
            Assert.AreEqual(process.CastLM(234.4m), vm.StartTestExecution<long>("CastLM", process, 234.4m));

            //float
            Assert.AreEqual(process.CastFB((byte)45), vm.StartTestExecution<float>("CastFB", process, (byte)45));
            Assert.AreEqual(process.CastFD(234.4d), vm.StartTestExecution<float>("CastFD", process, 234.4d));
            Assert.AreEqual(process.CastFI(45), vm.StartTestExecution<float>("CastFI", process, 45));
            Assert.AreEqual(process.CastFM(234.4m), vm.StartTestExecution<float>("CastFM", process, 234.4m));

            //dobule
            Assert.AreEqual(process.CastDB((byte)45), vm.StartTestExecution<double>("CastDB", process, (byte)45));
            Assert.AreEqual(process.CastDF(234.4f), vm.StartTestExecution<double>("CastDF", process, 234.4f));
            Assert.AreEqual(process.CastDI(45), vm.StartTestExecution<double>("CastDI", process, 45));
            Assert.AreEqual(process.CastDM(234.4m), vm.StartTestExecution<double>("CastDM", process, 234.4m));

            //short
            Assert.AreEqual(process.CastSB((byte)45), vm.StartTestExecution<short>("CastSB", process, (byte)45));
            Assert.AreEqual(process.CastSF(234.4f), vm.StartTestExecution<short>("CastSF", process, 234.4f));
            Assert.AreEqual(process.CastSI(45), vm.StartTestExecution<short>("CastSI", process, 45));
            Assert.AreEqual(process.CastSM(234.4m), vm.StartTestExecution<short>("CastSM", process, 234.4m));
            Assert.AreEqual(process.CastSD(234.4d), vm.StartTestExecution<short>("CastSD", process, 234.4d));

            //byte
            Assert.AreEqual(process.CastBI(45), vm.StartTestExecution<byte>("CastBI", process, 45));
            Assert.AreEqual(process.CastBF(234.4f), vm.StartTestExecution<byte>("CastBF", process, 234.4f));
            Assert.AreEqual(process.CastBD(234.4d), vm.StartTestExecution<byte>("CastBD", process, 234.4d));
            Assert.AreEqual(process.CastBM(234.4m), vm.StartTestExecution<byte>("CastBM", process, 234.4m));

            //Assert.Pass();
        }
    }
    public class CastingTestProcess
    {
        //Separate name to test only casting and not function overloading 
        public int CastIB(byte x) => (int)x;
        public int CastIF(float x) => (int)x;
        public int CastID(double x) => (int)x;
        public int CastIM(decimal x) => (int)x;
        public decimal CastMB(byte x) => (decimal)x;
        public decimal CastMF(float x) => (decimal)x;
        public decimal CastMD(double x) => (decimal)x;
        public decimal CastMI(int x) => (decimal)x;
        public long CastLI(int x) => (long)x;
        public long CastLM(decimal x) => (long)x;
        public long CastLD(double x) => (long)x;
        public long CastLF(float x) => (long)x;
        public long CastLB(byte x) => (long)x;
        public float CastFI(int x) => (float)x;
        public float CastFM(decimal x) => (float)x;
        public float CastFD(double x) => (float)x;
        public float CastFB(byte x) => (float)x;
        public double CastDI(int x) => (double)x;
        public double CastDF(float x) => (double)x;
        public double CastDM(decimal x) => (double)x;
        public double CastDB(byte x) => (double)x;
        public short CastSI(int x) => (short)x;
        public short CastSD(double x) => (short)x;
        public short CastSM(decimal x) => (short)x;
        public short CastSF(float x) => (short)x;
        public short CastSB(byte x) => (short)x;
        public short CastBI(int x) => (byte)x;
        public short CastBF(float x) => (byte)x;
        public short CastBD(double x) => (byte)x;
        public short CastBM(decimal x) => (byte)x;
    }
}
