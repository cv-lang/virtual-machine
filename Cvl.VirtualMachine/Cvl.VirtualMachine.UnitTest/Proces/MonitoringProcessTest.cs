using Cvl.VirtualMachine.Core.Attributes;
using Cvl.VirtualMachine.UnitTest.Proces.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Cvl.VirtualMachine.UnitTest.Proces
{
    public class MonitoringProcessTest
    {
        [Test]
        public void TestMethod1()
        {
            var proces = new MonitoringProcess();
            var vm = new VirtualMachine();
            var vmWynik = vm.StartTestExecution<object>("Start", proces);

            proces = new MonitoringProcess();
            var wynik = proces.Start();
            Assert.AreEqual(wynik, vmWynik);
        }
    }

    public class MonitoringProcess
    {
        private ApplicationMonitor monitor;
        public string Start()
        {
            monitor = new ApplicationMonitor();

            var reqest = new RequestContract();
            var response = GetVerification(reqest);
            return "Ok";
        }

        [Interpret]
        public ResponseContract GetVerification(RequestContract request)
        {
            var log = monitor.StartLogs(() => request, param4: IPHelper.CheckIp(), 
                externalId: request?.ApplicationId.ToString());

            int i = 0;
            i += request.ApplicationId;

            return new ResponseContract();
        }

    }

    internal class IPHelper
    {
        internal static object CheckIp()
        {
            return "localhost";
        }
    }
}
