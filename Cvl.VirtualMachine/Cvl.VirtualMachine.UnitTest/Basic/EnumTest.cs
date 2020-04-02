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

            Assert.AreEqual(process.GetEnum1(1), vm.Start<EnumTestStatus>("GetEnum1", process, 1));

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
    }



    public enum EnumTestStatus
    {
        Init,
        Progress,
        Complite
    }
}
