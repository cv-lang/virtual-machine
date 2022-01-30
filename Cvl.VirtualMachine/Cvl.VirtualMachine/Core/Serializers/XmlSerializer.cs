using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Polenter.Serialization;
using Polenter.Serialization.Core;

namespace Cvl.VirtualMachine.Core.Serializers
{
    public class XmlSerializer : ISerializer
    {
        private readonly IServiceProvider _serviceProvider;

        public XmlSerializer(IServiceProvider serviceProvider = null)
        {
            _serviceProvider = serviceProvider;
        }

        public T? Deserialize<T>(string xmlOfAnObject)
        {
            if (xmlOfAnObject.StartsWith("?"))
            {
                xmlOfAnObject = xmlOfAnObject.Remove(0, 1);
            }

            if (xmlOfAnObject.Equals("<null/>"))
            {
                return default;
            }

            //return JsonConvert.DeserializeObject(xmlOfAnObject);


            var serializer = new SharpSerializer();
            var instanceCreator = new SimpleInstanceCreator(_serviceProvider);
            serializer.InstanceCreator = instanceCreator;
            serializer.PropertyProvider.AttributesToIgnore.Clear();
            // remove default ExcludeFromSerializationAttribute for performance gain
            serializer.PropertyProvider.AttributesToIgnore.Add(typeof(XmlIgnoreAttribute));
            byte[] bajty = Encoding.UTF8.GetBytes(xmlOfAnObject);
            using (var ms = new MemoryStream(bajty))
            {
                object obiekt = serializer.Deserialize(ms);

                instanceCreator.RunDeserializationInicaializer();
                return (T)obiekt;
            }
        }

        public string Serialize(object? obj)
        {
            if (obj == null)
            {
                return "<null/>";
            }

            //string preserveReferenacesAll = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            //{
            //    PreserveReferencesHandling = PreserveReferencesHandling.All
            //});

            //return preserveReferenacesAll;

            //return JsonConvert.SerializeObject(obj);

            var serializer = new SharpSerializer();
            serializer.PropertyProvider.AttributesToIgnore.Clear();
            // remove default ExcludeFromSerializationAttribute for performance gain
            serializer.PropertyProvider.AttributesToIgnore.Add(typeof(XmlIgnoreAttribute));

            using (var ms = new MemoryStream())
            {
                serializer.Serialize(obj, ms);
                ms.Position = 0;
                byte[] bajty = ms.ToArray();
                return Encoding.UTF8.GetString(bajty, 0, bajty.Length);
            }
        }
    }
}
