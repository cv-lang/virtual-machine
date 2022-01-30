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
        private readonly IServiceProvider _serviceProvider;
        private List<IDeserializedObject> deserializedObjects = new List<IDeserializedObject>();

        public SimpleInstanceCreator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object CreateInstance(Type type)
        {
            object instance = null;

            if (_serviceProvider != null)
            {
                instance = _serviceProvider.GetService(type);
            }
            if (instance == null)
            {
                instance = Activator.CreateInstance(type);
            }

            if (instance is IDeserializedObject deserializedObject)
            {
                deserializedObjects.Add(deserializedObject);
            }

            return instance;
        }

        public void RunDeserializationInicaializer()
        {
            deserializedObjects.Reverse();
            foreach (var deserializedObject in deserializedObjects)
            {
                deserializedObject.AfterDeserialization();
            }
        }
    }
}
