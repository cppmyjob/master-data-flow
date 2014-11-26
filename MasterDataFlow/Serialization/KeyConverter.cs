using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;
using Newtonsoft.Json;

namespace MasterDataFlow.Serialization
{
    public class KeyConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var baseKey = (BaseKey)value;
            writer.WriteStartObject();
            writer.WritePropertyName("Key");
            writer.WriteValue(baseKey.Key);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonException("Unexpected TokenType :" + reader.TokenType);
            reader.Read();
            var propertyName = (string) reader.Value;
            if (propertyName != "Key")
                throw new JsonException("Unexpected JSON property name:" + propertyName);
            reader.Read();
            var key = (string)reader.Value;
            var result = BaseKey.DeserializeKey(key);
            reader.Read();
            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(BaseKey).IsAssignableFrom(objectType);
        }
    }
}
