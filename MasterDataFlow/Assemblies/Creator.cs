using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Assemblies
{
    public static class Creator
    {
        public static ICommandDataObject CreateDataObjectInstance(string dataObject, string dataObjectTypeName)
        {
            if (String.IsNullOrEmpty(dataObject))
                return null;
            var dataObjectType = Type.GetType(dataObjectTypeName);
            var result = (ICommandDataObject)Serialization.Serializator.Deserialize(dataObjectType, dataObject);
            return result;
        }

        public static BaseCommand CreateCommandInstance(CommandKey commandKey, string commandTypeName, ICommandDataObject commandDataObject)
        {
            var commandType = Type.GetType(commandTypeName);
            return CreateCommandInstance(commandKey, commandType, commandDataObject);
        }

        public static BaseCommand CreateCommandInstance(CommandKey commandKey, Type commandType, ICommandDataObject commandDataObject)
        {
            var instance = (BaseCommand) Activator.CreateInstance(commandType);

            instance.Key = commandKey;
            if (commandDataObject != null)
            {
                PropertyInfo dataObjectProperty = commandType.GetProperty("DataObject", BindingFlags.Instance | BindingFlags.Public);
                // TODO need to add a some checking is DataObject exist and etc
                dataObjectProperty.SetValue(instance, commandDataObject, null);
            }
            return instance;
        }
    }
}
