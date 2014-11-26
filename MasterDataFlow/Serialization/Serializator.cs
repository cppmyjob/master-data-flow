using System;
using System.Collections.Generic;
using System.Linq;
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
            var result = JsonConvert.SerializeObject(obj, new KeyConverter(), new CommandDataObjectConverter());
            return result;
        }

        public static object Deserialize(Type type, string value)
        {
            var result = JsonConvert.DeserializeObject(value, type, new KeyConverter(), new CommandDataObjectConverter());
            return result;
        }

        public static object DeserializeDataObject(Type type, string value, WorkflowKey workflowKey, IInstanceFactory instanceFactory)
        {
            var result = JsonConvert.DeserializeObject(value, type, new KeyConverter(), new CommandDataObjectConverter(workflowKey, instanceFactory));
            return result;
        }

    }
}
