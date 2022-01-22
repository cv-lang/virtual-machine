using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Core.Variables
{
    public class ObjectWraperBase : ElementBase
    {
        public virtual object GetValue()
        {
            return null;
        }

        public virtual void SetNull()
        {

        }

        public virtual void SetValue(object ret)
        {
            throw new NotImplementedException();
        }
    }
}
