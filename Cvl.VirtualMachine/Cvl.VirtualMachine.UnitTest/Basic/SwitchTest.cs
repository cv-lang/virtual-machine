using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.UnitTest.Basic
{
    public class SwitchTest
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void Test1()
        {
            var vm = new VirtualMachine();
            var process = new SwitchTestProcess();
            
            Assert.AreEqual(process.Switch(), vm.StartTestExecution<int>("Switch", process));
        }
    }

    public class SwitchTestProcess
    {
        public int Switch()
        {
            int ret = 0;
            for (int i = 0; i < 10; i++)
            {
                switch (i)
                {
                    case 1:
                        ret += 1;
                        break;
                    case 2:
                        ret += 2;
                        break;
                    case 3:
                        ret += 3;
                        break;
                    case 4:
                        ret += 4;
                        break;
                    case 5:
                        ret += 5;
                        break;
                    case 6:
                        ret += 6;
                        break;
                    case 7:
                        ret += 7;
                        break;
                    case 8:
                        ret += 8;
                        break;
                    case 9:
                        ret += 9;
                        break;
                }
            }
            return ret;
        }
    }
}
