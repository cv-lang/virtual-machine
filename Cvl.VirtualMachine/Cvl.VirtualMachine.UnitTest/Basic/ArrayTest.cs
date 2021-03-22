using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Basic
{
    public class ArrayTest
    {
        [SetUp]
        public void Setup() { }
        
        [Test]
        public void Test1()
        {
            var vm = new VirtualMachine();
            var process = new ArrayTestProcess();

            int[] testArray1 = { 2, 4, 6, 8, 10, 15, 20 };

            Assert.AreEqual(process.ReadIndex(testArray1, 3), vm.StartTestExecution<int>("ReadIndex", process, testArray1, 3));
            var ex = Assert.Throws<Exception>(() => vm.StartTestExecution<int>("ReadIndex", process, testArray1, 20));
            Assert.That(ex.InnerException, Is.TypeOf<IndexOutOfRangeException>());

            Assert.AreEqual(process.ChangeValueAtIndex(testArray1, 5, 30), vm.StartTestExecution<int>("ChangeValueAtIndex", process, testArray1, 5, 30));
            ex = Assert.Throws<Exception>(() => vm.StartTestExecution<int>("ChangeValueAtIndex", process, testArray1, 10, 30));
            Assert.That(ex.InnerException, Is.TypeOf<IndexOutOfRangeException>());
        }

    }

    public class ArrayTestProcess
    {
        public object ReadIndex(int[] array, int index)
        {
            return array[index];
        }

        public object ChangeValueAtIndex(int[] array, int index, int value)
        {
            array[index] = value;
            return array[index];
        }

        
    }
}
