using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cvl.VirtualMachine.UnitTest.Basic
{
    public class InstanceTest
    {
        [Test]
        public void Test1()
        {
            var vm = new VirtualMachine();
            Cvl.VirtualMachine.Test.VirtualMachineDebug.VirtualMachine = vm;
            var process = new ServiceTest();

            var r1 = process.Start();
            var r2 = vm.StartTestExecution<int>("Start", process);

            Assert.AreEqual(r1, r2);
        }
    }

    public class ServiceTest
    {
        private LoggerFactory loggerFactory { get; set; } = new LoggerFactory();
        public int iter { get; set; }

        public int Start()
        {
            iter = 1;
            var request = new Request();

            using (var logger = GetLogger("", ""))
            {
                logger.AddParameter(request, "request");
            }

            return iter;
        }

        public Logger GetLogger(object externalId1 = null, object external2 = null, object external3 = null, object external4 = null, string message = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {

            //jeśli brak logera to tworzę nowego logera
            var logger = loggerFactory.GetLogger(externalId1?.ToString(), external2?.ToString(), external3?.ToString(), external4?.ToString(), message, memberName, sourceFilePath,
                sourceLineNumber);

            iter += sourceLineNumber;

            return logger;
        }
    }

    public class LoggerFactory
    {
        internal Logger GetLogger(string v1, string v2, string v3, string v4, string message, string memberName, string sourceFilePath, int sourceLineNumber)
        {
            return new Logger();
        }
    }

    public class Logger : IDisposable
    {
        public string TestPrperty { get; set; }

        public void Dispose()
        {            
        }

        internal Logger AddParameter(Request request, string v)
        {
            return this;
        }
    }

    public class Request
    {
        public string TestPrperty1 { get; set; }
    }
}
