using Cvl.VirtualMachine.Core.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.UnitTest.Basic
{
    public class InheritanceTest
    {
        [Test]
        public void Test1()
        {
            var vm = new VirtualMachine();
            var process = new InheritanceProcesTest();

            var r1 = vm.StartTestExecution<int>("Start", process);

            Assert.AreEqual(process.Start(), (object)r1);
        }
    }

    public class InheritanceProcesTest
    {
        public int Start()
        {
            int i = 1;

            A a = null;
            B b = new B();
            C c = new C();

            i += b.GetValue();
            i += c.GetValue();

            a = b;
            i += a.GetValue();

            a = c;
            i += a.GetValue();

            b = c;
            i += b.GetValue();

            return i;
        }
    }

    public abstract class A
    {
        [Interpret]
        public abstract int GetValue();
    }

    public class B : A
    {
        public override int GetValue()
        {
            return 2;
        }
    }

    public class C : B
    {
        public override int GetValue()
        {
            return 3;
        }
    }
}
