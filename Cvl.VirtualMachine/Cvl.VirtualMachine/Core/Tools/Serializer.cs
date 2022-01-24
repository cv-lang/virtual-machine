//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using System.Xml.Serialization;
//using Newtonsoft.Json;
//using Polenter.Serialization;

//namespace Cvl.VirtualMachine.Core.Tools
//{
//    public class Serializer
//    {
//        #region Serialisacha SharpSerializer

//        public static string SerializeObject(object obj)
//        {
//            if (obj == null)
//            {
//                return "<null/>";
//            }

//            //string preserveReferenacesAll = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
//            //{
//            //    PreserveReferencesHandling = PreserveReferencesHandling.All
//            //});

//            //return preserveReferenacesAll;

//            //return JsonConvert.SerializeObject(obj);

//            var serializer = new SharpSerializer();
//            serializer.PropertyProvider.AttributesToIgnore.Clear();
//            // remove default ExcludeFromSerializationAttribute for performance gain
//            serializer.PropertyProvider.AttributesToIgnore.Add(typeof(XmlIgnoreAttribute));

//            using (var ms = new MemoryStream())
//            {
//                serializer.Serialize(obj, ms);
//                ms.Position = 0;
//                byte[] bajty = ms.ToArray();
//                return Encoding.UTF8.GetString(bajty, 0, bajty.Length);
//            }
//        }


//        public static T DeserializeObject<T>(string xmlOfAnObject)
//            where T : class
//        {
//            if (xmlOfAnObject.StartsWith("?"))
//            {
//                xmlOfAnObject = xmlOfAnObject.Remove(0, 1);
//            }

//            if (xmlOfAnObject.Equals("<null/>"))
//            {
//                return null;
//            }

//            //return JsonConvert.DeserializeObject<T>(xmlOfAnObject);

//            return (T)DeserializeObject(xmlOfAnObject);
//        }

//        public static object DeserializeObject(string xmlOfAnObject)
//        {
//            if (xmlOfAnObject.StartsWith("?"))
//            {
//                xmlOfAnObject = xmlOfAnObject.Remove(0, 1);
//            }

//            if (xmlOfAnObject.Equals("<null/>"))
//            {
//                return null;
//            }

//            //return JsonConvert.DeserializeObject(xmlOfAnObject);


//            var serializer = new SharpSerializer();
//            serializer.PropertyProvider.AttributesToIgnore.Clear();
//            // remove default ExcludeFromSerializationAttribute for performance gain
//            serializer.PropertyProvider.AttributesToIgnore.Add(typeof(XmlIgnoreAttribute));
//            byte[] bajty = Encoding.UTF8.GetBytes(xmlOfAnObject);
//            using (var ms = new MemoryStream(bajty))
//            {
//                object obiekt = serializer.Deserialize(ms);

//                return obiekt;
//            }
//        }




//        #endregion

//    }


//    public class SerializerJson
//    {
//        #region Serialisacha SharpSerializer

//        public static string SerializeObject(object obj)
//        {
//            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
//            var text = JsonConvert.SerializeObject(obj, settings);
//            return text;
//        }

//        public static object DeserializeObject(string json)
//        {
//            var settings = new JsonSerializerSettings
//            {
//                TypeNameHandling = TypeNameHandling.All,
//                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
//            };
//            return JsonConvert.DeserializeObject<object>(json, settings);
//        }

//        public static T DeserializeObject<T>(string json)
//        {
//            var settings = new JsonSerializerSettings
//            {
//                TypeNameHandling = TypeNameHandling.All,
//                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
//            };
//            return JsonConvert.DeserializeObject<T>(json, settings);
//        }


//        #endregion

//    }
//}
