using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.UnitTest.Proces.Model
{
    public class ObiektBiznesowy
    {
        public Guid Id { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var ob = obj as ObiektBiznesowy;
            if (ob != null)
            {
                return ob.Id == Id && ob.GetType() == this.GetType();
            }
            else
            {
                return base.Equals(obj);
            }
        }
    }
}
