using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.UnitTest.Proces.Model
{
    public class Person : ObiektBiznesowy
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }

        public override bool Equals(object obj)
        {
            var p = obj as Person;
            return Surname == p.Surname && Name == p.Name && Age == p.Age && Address == p.Address;
        }
    }
}
