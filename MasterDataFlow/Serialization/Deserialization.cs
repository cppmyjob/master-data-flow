using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MasterDataFlow.Interfaces;
using Newtonsoft.Json;

namespace MasterDataFlow.Serialization
{
    public static class Deserialization
    {
        public static object DeserializateDataObject(Type type,string value)
        {
            var result = JsonConvert.DeserializeObject(value, type);
            return result;
        }
    }
}
