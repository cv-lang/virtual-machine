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
            Assert.AreEqual(process.CastLB((byte)45), vm.StartTestExecution<long>("CastMB", process, (byte)45));
            Assert.AreEqual(process.CastLF(234.4f), vm.StartTestExecution<long>("CastMF", process, 234.4f));
            Assert.AreEqual(process.CastLD(234.4d), vm.StartTestExecution<long>("CastMD", process, 234.4d));
            Assert.AreEqual(process.CastLI(45), vm.StartTestExecution<long>("CastMI", process, 45));
            Assert.AreEqual(process.CastLM(234.4m), vm.StartTestExecution<long>("CastIM", process, 234.4m));

            //float
            Assert.AreEqual(process.CastFB((byte)45), vm.StartTestExecution<float>("CastMB", process, (byte)45));
            Assert.AreEqual(process.CastFD(234.4d), vm.StartTestExecution<float>("CastMD", process, 234.4d));
            Assert.AreEqual(process.CastFI(45), vm.StartTestExecution<float>("CastMI", process, 45));
            Assert.AreEqual(process.CastFM(234.4m), vm.StartTestExecution<float>("CastIM", process, 234.4m));

            //dobule
            Assert.AreEqual(process.CastDB((byte)45), vm.StartTestExecution<double>("CastMB", process, (byte)45));
            Assert.AreEqual(process.CastDF(234.4f), vm.StartTestExecution<double>("CastMD", process, 234.4f));
            Assert.AreEqual(process.CastDI(45), vm.StartTestExecution<double>("CastMI", process, 45));
            Assert.AreEqual(process.CastDM(234.4m), vm.StartTestExecution<double>("CastIM", process, 234.4m));

            //short
            Assert.AreEqual(process.CastSB((byte)45), vm.StartTestExecution<short>("CastMB", process, (byte)45));
            Assert.AreEqual(process.CastSF(234.4f), vm.StartTestExecution<short>("CastMD", process, 234.4f));
            Assert.AreEqual(process.CastSI(45), vm.StartTestExecution<short>("CastMI", process, 45));
            Assert.AreEqual(process.CastSM(234.4m), vm.StartTestExecution<short>("CastIM", process, 234.4m));
            Assert.AreEqual(process.CastSD(234.4d), vm.StartTestExecution<short>("CastIM", process, 234.4d));

            //byte
            Assert.AreEqual(process.CastBI(45), vm.StartTestExecution<byte>("CastMB", process, 45));
            Assert.AreEqual(process.CastBF(234.4f), vm.StartTestExecution<byte>("CastMD", process, 234.4f));
            Assert.AreEqual(process.CastBD(234.4d), vm.StartTestExecution<byte>("CastMI", process, 234.4d));
            Assert.AreEqual(process.CastBM(234.4m), vm.StartTestExecution<byte>("CastIM", process, 234.4m));

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
