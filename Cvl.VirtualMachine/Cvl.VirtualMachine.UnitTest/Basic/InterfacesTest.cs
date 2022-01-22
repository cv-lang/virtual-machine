using Cvl.VirtualMachine.Core.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.UnitTest.Basic
{
    public class InterfacesTest
    {
        [Test]
        public void Test1()
        {
            var vm = new VirtualMachine();
            Cvl.VirtualMachine.Test.VirtualMachineDebug.VirtualMachine = vm;
            var process = new InterfacesProcessTest();

            var r1 = process.Start();
            var r2 = vm.StartTestExecution<int>("Start", process);

            Assert.AreEqual(r1, r2);
        }
    }

    public class InterfacesProcessTest
    {
        public int Start()
        {
            int i = 1;
            string data = "sdfsdf";
            IA a = null;
            a = new A1();
            i += a.GetValue(data , 1);

            a = new A2();
            i += a.GetValue(data, 1);

            return i;
        }
    }

    public interface IA
    {
        [Interpret]
        int GetValue(string data, int i);
    }

    public class A1 : IA
    {
        [Interpret]
        public int GetValue(string data, int i)
        {
            return data.Length + i + 1;
        }
    }

    public class A2 : IA
    {
        [Interpret]
        public int GetValue(string data, int i)
        {
            return data.Length + i + 2;
        }
    }
}
