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
            var serializer = new Cvl.VirtualMachine.Core.Serializers.XmlSerializer();

            var proces = new HibernateTestProcess();
            var vm = new VirtualMachine();
            Cvl.VirtualMachine.Test.VirtualMachineDebug.VirtualMachine = vm;
            vm.Instance = proces;

            var xml = serializer.Serialize(vm);

            var vmWynik = vm.Start<object>("Start", proces);
            Assert.True(vmWynik.State == VirtualMachineState.Hibernated);
            Assert.True(proces.State == 1);

            //vm.Thread.AktualnaMetoda = null;

            xml = serializer.Serialize(vm);
            var vm2 = serializer.Deserialize<VirtualMachine>(xml);
            vm = vm2;

            vmWynik = vm.Resume<object>();
            Assert.True(vmWynik.State == VirtualMachineState.Hibernated);
            Assert.True(proces.State == 2);

            vmWynik = vm.Resume<object>();
            Assert.True(vmWynik.State == VirtualMachineState.Executed);
            Assert.True(proces.State == 3);

            proces = new HibernateTestProcess();
            var wynik = proces.Start();
            Assert.AreEqual(wynik, vmWynik.Result);
        }       

    }

    public class HibernateTestProcess
    {
        public int State { get; set; }
        
        public object Start()
        {
            int i = 2;
            State++;
            VirtualMachine.Hibernate();
            State++;
            VirtualMachine.Hibernate();
            State++;
            

            return State;
        }
    }
}
