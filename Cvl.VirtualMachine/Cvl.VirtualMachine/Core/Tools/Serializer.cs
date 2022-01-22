using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Cvl.VirtualMachine.Core.Tools
{
    public class Serializer
    {
        #region Serialisacha SharpSerializer

        public static string SerializeObject(object obj)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var text = JsonConvert.SerializeObject(obj, settings);
            return text;
        }

        public static object DeserializeObject(string json)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
            };
            return JsonConvert.DeserializeObject<object>(json, settings);
        }

        public static T DeserializeObject<T>(string json)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
            };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }


        #endregion

    }
}
