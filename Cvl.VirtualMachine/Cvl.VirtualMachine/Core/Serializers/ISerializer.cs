using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cvl.VirtualMachine.Core.Serializers
{
    public interface ISerializer
    {
        string Serialize(object? obj);

        T? Deserialize<T>(string serializedForm);
    }
}
