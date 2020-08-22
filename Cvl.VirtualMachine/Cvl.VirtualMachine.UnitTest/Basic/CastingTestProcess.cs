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
    }
}
