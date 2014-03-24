using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MasterDataFlow.Serialization
{
    public static class Serializator
    {
        public static string Serialize(object obj)
        {
            var result = JsonConvert.SerializeObject(obj);
            return result;
        }
    }
}
