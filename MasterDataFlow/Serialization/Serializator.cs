using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static object DeserializeDataObject(Type type, string value, WorkflowKey workflowKey, IInstanceFactory instanceFactory)
        {
            return DeserializeObject(value);
            //var result = JsonConvert.DeserializeObject(value, type, new KeyConverter(), new CommandDataObjectConverter(workflowKey, instanceFactory));
            //return result;
        }

        public static string SerializeObject(object data)
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

        public static object DeserializeObject(string data)
        {
            BinaryFormatter bformatter = new BinaryFormatter();
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
