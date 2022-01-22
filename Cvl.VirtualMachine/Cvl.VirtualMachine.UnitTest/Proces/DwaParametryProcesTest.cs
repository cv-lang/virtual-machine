using Cvl.VirtualMachine.UnitTest.Proces.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cvl.VirtualMachine.UnitTest.Proces
{
    public class DwaParametryProcesUniteTest
    {
        [Test]
        public void DwaParametryProcesTest()
        {
            var proces = new DwaParametryProces();
            var vm = new VirtualMachine();
            var vmWynik = vm.StartTestExecution<object>("Start", proces);

            proces = new DwaParametryProces();
            var wynik = proces.Start();
            Assert.AreEqual(wynik, vmWynik);
        }


        [Test]
        public void DodawaniePracownikaProcesTest()
        {
            var proces = new DodawaniePracownikaProces();
            var vm = new VirtualMachine();
            var vmWynik = vm.StartTestExecution<object>("Start", proces);

            proces = new DodawaniePracownikaProces();
            var wynik = proces.Start();
            Assert.IsTrue(wynik.Equals(vmWynik));
        }

        [Test]
        public void RepozytoriumTestProces()
        {
            var proces = new RepozytoriumTestProces();
            var vm = new VirtualMachine();
            var vmWynik = (List<Person>)vm.StartTestExecution<object>("Start", proces);

            proces = new RepozytoriumTestProces();
            var wynik = (List<Person>)proces.Start();
            Assert.IsTrue(wynik.Count == vmWynik.Count && wynik[0].Equals(vmWynik[0]));
        }

        [Test]
        public void StartAsLambdaProcess()
        {
            var proces = new RepozytoriumTestProces();
            var vm = new Cvl.VirtualMachine.VirtualMachine();
            vm.InterpreteFullNameTypes = "Cvl.VirtualMachine.Test";
            List<Person> response = null;
            vm.Start(() =>
            {
                response = proces.StartWithParameters(1,"2");
            });

            var proces2 = new RepozytoriumTestProces();
            var response2 = proces2.StartWithParameters(1, "2");

            Assert.IsTrue(response.Count == response2.Count);
            for (int i = 0; i < response.Count; i++)
            {
                var r1 = response[i];
                var r2 = response2[i];
                Assert.AreEqual(r1.Age, r2.Age);
                Assert.AreEqual(r1.Name, r2.Name);
            }
        }
    }


    public class DwaParametryProces
    {
        public DwaParametryProces()
        {

        }

        public DwaParametryProces(int i, DateTime data)
        {
            Data = data;
        }

        public DateTime Data { get; set; }

        public object Start()
        {
            var proces = new DwaParametryProces(4, DateTime.Today);

            return proces.Data;
        }
    }

    public class DodawaniePracownikaProces
    {
        public object Start()
        {
            var prac = new Person();
            prac.Surname = "Kowalski";
            prac.Name = "Jan";

            return prac;
        }
    }

    public class RepozytoriumTestProces
    {
        public object Start()
        {
            var rep = new RepozytoriumTypowane<Person>();
            List<Person> pracownicy = rep.PobierzObiektyTypowane();

            int i = 0;
            foreach (var pracownik in pracownicy)
            {
                pracownik.Name = "Prac " + i;
                i++;
            }
            return pracownicy;
        }

        public List<Person> StartWithParameters(int typeId, string name)
        {
            var rep = new RepozytoriumTypowane<Person>();
            List<Person> pracownicy = rep.PobierzObiektyTypowane();

            int i = 0;
            foreach (var pracownik in pracownicy)
            {
                pracownik.Name = "Prac " + i + " " + typeId + name;
                pracownik.Age = typeId;
                i++;
            }
            return pracownicy;
        }
    }
}
