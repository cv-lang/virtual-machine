using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cvl.VirtualMachine.Core.Serializers
{
    public interface IDeserializedObject
    {
        void AfterDeserialization();
    }

    /// <summary>
    /// Obiekt stanu korzystający z usług z kontenera DI. Usługi trzyma w polach [NonSerialized],
    /// a dostaje je po odczycie, przed wywołaniami AfterDeserialization.
    /// </summary>
    public interface IUsesServices
    {
        void AttachServices(IServiceProvider services);
    }
}
