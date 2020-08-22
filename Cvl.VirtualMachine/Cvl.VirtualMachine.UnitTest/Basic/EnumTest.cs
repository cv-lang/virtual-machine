using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.UnitTest.Basic
{
    public class EnumTest
    {
        [Test]
        public void Test1()
        {
            var vm = new VirtualMachine();
            var process = new EnumProces();

            Assert.AreEqual(process.GetEnum1(1), vm.StartTestExecution<EnumTestStatus>("GetEnum1", process, 1));


        }

        [Test]
        public void Test2()
        {
            var vm = new VirtualMachine();
            var process = new EnumProces();

            var ret = vm.StartTestExecution<int>("Test2", process,2);
            Assert.AreEqual(process.Test2(2), ret);


        }
    }

    public class EnumProces
    {
        public EnumTestStatus GetEnum1(int i)
        {
            var status = EnumTestStatus.Init;

            status = (EnumTestStatus)i;

            //if(status == EnumTestStatus.Init)
            //{
            //    return EnumTestStatus.Init;
            //}

            //if(status == EnumTestStatus.Progress)
            //{
            //    status = EnumTestStatus.Init;
            //}

            //if(status == EnumTestStatus.Complite)
            //{
            //    return EnumTestStatus.Complite;
            //}

            var stat2 = getStat();
            if(stat2 == EnumTestStatus.Init)
            {
                return stat2;
            } else if(stat2 == EnumTestStatus.Complite)
            {
                return status;
            }


            return status;
        }


        public EnumTestStatus getStat()
        {
            return EnumTestStatus.Init;
        }


        public int Test2(int ruleTypeId)
        {
            var t1 = ruleTypeId == (int) RuleTypeEnum.ToDecission ? (int) RuleTypeEnum.ToChooseProcess : ruleTypeId;
            return t1;
        }

        
    }

    public enum RuleTypeEnum
    {
        Initial = 1,
        ToDecission = 2,
        ToChooseProcess = 3
    }

    public enum EnumTestStatus
    {
        Init,
        Progress,
        Complite
    }
}
