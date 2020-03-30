using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.UnitTest.Proces
{
    public class ArithmeticProcess
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double XX { get; set; }

        public object Start()
        {
            int i = 2;
            XX = 1;
            double sum = 0;

            for (int j = 0; j < 10; j++)
            {
                X = j * Y + j / i;
                XX = 2.0 * j + XX / 2.0;
                XX = XX / (XX + 2.0);
                sum += XX;
            }

            return sum;
        }
    }

    
    public class AritheticProcessUniteTest
    {
        [Test]
        public void ArithmeticTest()
        {
            var proces = new ArithmeticProcess();
            var vm = new VirtualMachine();
            var vmWynik = vm.Start<object>("Start", proces);
            
            proces = new ArithmeticProcess();
            var wynik = proces.Start();
            Assert.AreEqual(wynik, vmWynik);
        }
    }
}
