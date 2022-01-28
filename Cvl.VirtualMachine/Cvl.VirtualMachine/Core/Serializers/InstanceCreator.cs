using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polenter.Serialization.Core;

namespace Cvl.VirtualMachine.Core.Serializers
{
    public class SimpleInstanceCreator : IInstanceCreator
    {
        public object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}
