using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.UnitTest.Proces.FromLife
{
    public class CheclRulesTest
    {
        [Test]
        public void Test1()
        {
            var proces = new ChcekRulesProcessTest();
            var vm = new VirtualMachine();
            var vmWynik = vm.Start<string>("Start", proces);

            proces = new ChcekRulesProcessTest();
            var wynik = proces.Start();
            Assert.AreEqual(wynik, vmWynik);
        }
    }

    public class ChcekRulesProcessTest
    {
        public string Start()
        {
            var request = new RequestTest(){Id = 3244};
            return GetCheck(request);
        }

        public string GetCheck(RequestTest request)
        {
            return CheckRules(request.Id, 8, 3, 3);
        }

        public string CheckRules(int? applicationEngineId, int applicationId, int applicationTypeId, int ruleTypeId)
        {
            var str1 = $"ruleTypeId:{ruleTypeId}";

            var str2 = $"{this} {applicationEngineId} {applicationId} {applicationTypeId} {ruleTypeId}";

            return str1 + str2;
        }
    }

    public class RequestTest
    {
        public int Id { get; set; }
    }
}
