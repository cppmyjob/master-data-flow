using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using Newtonsoft.Json;

namespace MasterDataFlow.Serialization
{
    public class CommandDataObjectConverter : JsonConverter
    {
        private readonly WorkflowKey _workflowKey;
        private readonly IInstanceFactory _instanceFactory;

        public CommandDataObjectConverter()
        {
            
        }

        public CommandDataObjectConverter(WorkflowKey workflowKey, IInstanceFactory instanceFactory)
        {
            _workflowKey = workflowKey;
            _instanceFactory = instanceFactory;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(value.GetType().AssemblyQualifiedName);
            writer.WritePropertyName("Data");
            var data = JsonConvert.SerializeObject(value, new KeyConverter());
            writer.WriteValue(data);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonException("Unexpected TokenType :" + reader.TokenType);
            reader.Read();

            var typeNamePropertyName = (string)reader.Value;
            if (typeNamePropertyName != "Type")
                throw new JsonException("Unexpected JSON property name:" + typeNamePropertyName);
            reader.Read();

            var typeName = (string)reader.Value;
            Type type;
            if (_workflowKey == null)
                type = Type.GetType(typeName);
            else
                type = _instanceFactory.GetType(_workflowKey, typeName);
            reader.Read();

            var dataPropertyName = (string)reader.Value;
            if (dataPropertyName != "Data")
                throw new JsonException("Unexpected JSON property name:" + dataPropertyName);
            reader.Read();

            var data = (string)reader.Value;
            reader.Read();
            var result = JsonConvert.DeserializeObject(data, type, new KeyConverter());
            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ICommandDataObject).IsAssignableFrom(objectType);
        }
    }
}
