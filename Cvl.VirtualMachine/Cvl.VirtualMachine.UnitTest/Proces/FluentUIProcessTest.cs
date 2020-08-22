using Cvl.VirtualMachine.UnitTest.Proces.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Cvl.VirtualMachine.UnitTest.Proces
{
    public class FluentUITest
    {
        [Test]
        public void TestMethod1()
        {
            var proces = new FluentUIProcess();
            var vm = new VirtualMachine();
            var vmWynik = vm.StartTestExecution<object>("Start",proces);
            
            proces = new FluentUIProcess();
            var wynik = proces.Start();
            Assert.AreEqual(wynik, vmWynik);
        }
    }

    public class FluentUIProcess
    {
        public object Wynik { get; set; }

        public object Start()
        {
            var view = DataFormView<Person>();
            view.DataForm(df =>
            {
                df.AddField(p => p.Name);
                df.AddField(p => p.Surname);
                df.AddField(p => p.Age);
            });

            Wynik = view;

            return "OK";
        }

        public DataFormFactory<T> DataFormView<T>()
        {
            var view = new DataFormFactory<T>();

            return view;
        }
    }

    

    

    


}
