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
        private List<IDeserializedObject> deserializedObjects = new List<IDeserializedObject>();

        public object CreateInstance(Type type)
        {
            var obj =  Activator.CreateInstance(type);

            if (obj is IDeserializedObject deserializedObject)
            {
                deserializedObjects.Add(deserializedObject);
            }

            return obj;
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
