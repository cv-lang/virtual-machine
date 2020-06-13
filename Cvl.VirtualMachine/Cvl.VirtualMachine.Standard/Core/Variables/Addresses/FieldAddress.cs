using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Cvl.VirtualMachine.Core.Variables.Addresses
{
    public class FieldAddress : ObjectAddressWraper
    {
        public FieldInfo Field { get; set; }
        public object Object { get; set; }

        public override object GetValue()
        {
            var val = Field.GetValue(Object);            
            return val;
        }
    }
}
