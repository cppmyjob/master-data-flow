using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using Newtonsoft.Json;

namespace MasterDataFlow.Serialization
{
    public static class Serializator
    {
        public static string Serialize(object obj)
        {
            return SerializeObject(obj);
            //var result = JsonConvert.SerializeObject(obj, new KeyConverter(), new CommandDataObjectConverter());
            //return result;
        }

        public static object Deserialize(Type type, string value)
        {
            return DeserializeObject(value);
            //var result = JsonConvert.DeserializeObject(value, type, new KeyConverter(), new CommandDataObjectConverter());
            //return result;
        }

        private static string SerializeObject(object data)
        {
            BinaryFormatter bformatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                bformatter.Serialize(stream, data);
                byte[] buffer = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(buffer, 0, buffer.Length);
                return Convert.ToBase64String(buffer);
            }
        }


        sealed class PreMergeToMergedDeserializationBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                //Type typeToDeserialize = null;

                //// For each assemblyName/typeName that you want to deserialize to
                //// a different type, set typeToDeserialize to the desired type.
                //String exeAssembly = Assembly.GetExecutingAssembly().FullName;


                //// The following line of code returns the type.
                //typeToDeserialize = Type.GetType(String.Format("{0}, {1}",
                //    typeName, exeAssembly));
                var typeToDeserialize = Type.GetType(typeName);
                return typeToDeserialize;
            }
        }

        private static object DeserializeObject(string data)
        {
            BinaryFormatter bformatter = new BinaryFormatter();
            //bformatter.Binder = new PreMergeToMergedDeserializationBinder();
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] buffer = Convert.FromBase64String(data);
                stream.Write(buffer, 0, buffer.Length);
                stream.Position = 0;
                return bformatter.Deserialize(stream);
            }
        }

    }
}
