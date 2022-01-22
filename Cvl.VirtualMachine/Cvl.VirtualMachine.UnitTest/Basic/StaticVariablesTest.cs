using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Basic
{
    public class StaticVariablesTest
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void Test1()
        {
            var vm = new VirtualMachine();
            var process = new StaticVariablesTestProcess();



            Assert.AreEqual(process.ReadStatic(), vm.StartTestExecution<int>("ReadStatic", process));
            Assert.AreEqual(process.SaveStatic(5), vm.StartTestExecution<int>("SaveStatic", process, 5));
        }
    }

    public class StaticVariablesTestProcess
    {
        public object ReadStatic()
        {
            var stvar = new StaticVariable();
            var result = stvar.GetVar();
            return result;
        }

        public object SaveStatic(int value)
        {
            var stvar = new StaticVariable();
            var result = stvar.SaveVar(value);
            return result;
        }
    }

    public class StaticVariable
    {
        public static int stvar = 10;
        public int GetVar()
        {
            return stvar;
        }
        public int SaveVar(int value)
        {
            stvar = value;
            return stvar;
        }
    }
}
