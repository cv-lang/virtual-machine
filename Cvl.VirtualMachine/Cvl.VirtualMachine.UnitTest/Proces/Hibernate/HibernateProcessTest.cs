using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Cvl.VirtualMachine.Core;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Proces.Hibernate
{
    class HibernateProcessTest
    {
        [Test]
        public void DwaParametryProcesTest()
        {
            var proces = new HibernateTestProcess();
            var vm = new VirtualMachine();
            Cvl.VirtualMachine.Test.VirtualMachineDebug.VirtualMachine = vm;
            vm.Instance = proces;

            var xml = VirtualMachine.SerializeVirtualMachine(vm);

            var vmWynik = vm.Start<object>("Start", proces);
            Assert.True(vmWynik.State == VirtualMachineState.Hibernated);
            Assert.True(proces.State == 1);
            
            //vm.Thread.AktualnaMetoda = null;

            xml = VirtualMachine.SerializeVirtualMachine(vm);
            var vm2 = VirtualMachine.DeserializeVirtualMachine(xml);
            vm = vm2;

            vmWynik = vm.Resume<object>("param1");
            Assert.True(vmWynik.State == VirtualMachineState.Hibernated);
            //Assert.True(((HibernateTestProcess)vm.Instance).State == 7);

            vmWynik = vm.Resume<object>();
            Assert.True(vmWynik.State == VirtualMachineState.Executed);
            //Assert.True(((HibernateTestProcess)vm.Instance).State == 8);

            proces = new HibernateTestProcess();
            var wynik = proces.Start();
            Assert.AreEqual(wynik, vmWynik.Result);
        }       

    }

    public class HibernateTestProcess
    {
        public int State { get; set; }

        public string TestString { get; set; } = "Test string";
        
        public object Start()
        {
            int i = 2;
            State++;
            object testObject = 3;
            TestString = $"asdfasd { testObject}";
            var result1 = VirtualMachine.Hibernate("param1");
            CheckHibernationResult(result1);

            //TestString = $"asdfasd {3}";

            State += TestString.Length;

            VirtualMachine.Hibernate();
            State++;
            

            return State;
        }

        private void CheckHibernationResult(object o)
        {

        }
    }
}
