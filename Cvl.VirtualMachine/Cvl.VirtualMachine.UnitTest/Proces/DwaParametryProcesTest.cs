using Cvl.VirtualMachine.UnitTest.Proces.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
            var vmWynik = vm.Start<object>("Start", proces);

            proces = new DwaParametryProces();
            var wynik = proces.Start();
            Assert.AreEqual(wynik, vmWynik);
        }


        [Test]
        public void DodawaniePracownikaProcesTest()
        {
            var proces = new DodawaniePracownikaProces();
            var vm = new VirtualMachine();
            var vmWynik = vm.Start<object>("Start", proces);

            proces = new DodawaniePracownikaProces();
            var wynik = proces.Start();
            Assert.IsTrue(wynik.Equals(vmWynik));
        }

        [Test]
        public void RepozytoriumTestProces()
        {
            var proces = new RepozytoriumTestProces();
            var vm = new VirtualMachine();
            var vmWynik = vm.Start<object>("Start", proces);

            proces = new RepozytoriumTestProces();
            var wynik = proces.Start();
            Assert.IsTrue(wynik.Equals(vmWynik));
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
            var pracownicy = rep.PobierzObiektyTypowane();

            int i = 0;
            foreach (var pracownik in pracownicy)
            {
                pracownik.Name = "Prac " + i;
                i++;
            }
            return pracownicy;
        }
    }
}
