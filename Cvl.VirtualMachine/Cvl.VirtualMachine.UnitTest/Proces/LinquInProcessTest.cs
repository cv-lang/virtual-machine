using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cvl.VirtualMachine.Core.Attributes;
using Cvl.VirtualMachine.UnitTest.Proces.Model;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Proces
{
    public class LinquInProcessTest
    {
        [Test]
        public void TestMethod1()
        {
            var proces = new LinquInProcess();
            var vm = new VirtualMachine();
            var vmWynik = vm.Start<object>("Start", proces);

            proces = new LinquInProcess();
            var wynik = proces.Start();
            Assert.AreEqual(wynik, vmWynik);
        }
    }

    public class LinquInProcess
    {

        public int Start()
        {
            var list = createDataList();
            var person1 = list.First(x => x.Age == 34);
            var personLast = list.Where(x => x.Age == 34).OrderBy(x=> x.Name).Last();
            var listOlder = list.Where(x => x.Age <= person1.Age);
            listOlder = listOlder.OrderByDescending(x => x.Name);
            listOlder = listOlder.Where(x => x.Id < 10);

            var olders = listOlder.ToList();

            foreach (var person in listOlder)
            {
                person.Age++;
            }

            return person1.Age;
        }

        //[Interpret]
        private List<Person> createDataList()
        {
            var persons = new List<Person>();
            persons.Add(new Person(){ Id =1, Name = "Jan", Surname = "Kowalski", Age = 32});
            persons.Add(new Person() { Id=2, Name = "Adam", Surname = "Nowak", Age = 34 });
            persons.Add(new Person() { Id=3, Name = "Jan", Surname = "Nowowsky", Age = 24 });

            return persons;
        }
    }
}
