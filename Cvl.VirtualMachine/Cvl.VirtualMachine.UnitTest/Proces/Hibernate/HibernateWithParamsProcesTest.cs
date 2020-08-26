using System;
using System.Collections.Generic;
using System.Text;
using Cvl.VirtualMachine.Core;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Proces.Hibernate
{
    
    class HibernateWithParamsProcesTest
    {
        [Test]
        public void DwaParametryProcesTest()
        {
            var proces = new HibernateWithPrarametersTestProcess();
            var vm = new VirtualMachine();
            var vmWynik = vm.Start<object>("Start", proces);
            Assert.True(vmWynik.State == VirtualMachineState.Hibernated);
            Assert.True(proces.State == 1);
            var p1 = vm.GetHibernateParams();
            Assert.True(p1[0].Equals("parameter from vw process"));
            Assert.True(p1[1].Equals(proces.State));


            vmWynik = vm.Resume<object>("Parameter from host to vw process");
            Assert.True(vmWynik.State == VirtualMachineState.Hibernated);
            Assert.True(proces.State == 2);

            var p2 = vm.GetHibernateParams();
            Assert.True(p2[0].Equals("Parameter from host to vw process"));

            vmWynik = vm.Resume<object>();
            Assert.True(vmWynik.State == VirtualMachineState.Executed);
            Assert.True(proces.State == 3);

            proces = new HibernateWithPrarametersTestProcess();
            var wynik = proces.Start();
            Assert.AreEqual(wynik, vmWynik.Result);
        }
    }

    public class HibernateWithPrarametersTestProcess
    {
        public int State { get; set; }

        public object Start()
        {
            int i = 2;
            State++;
            var ret =VirtualMachine.Hibernate("parameter from vw process", State);
            State++;
            VirtualMachine.Hibernate(ret);
            State++;


            return State;
        }
    }
}
