using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;

namespace MasterDataFlow
{
    public class CommandDefinition
    {
        private Type _command;

        public CommandDefinition(Type command)
        {
            _command = command;
        }

        internal Type Command
        {
            get { return _command; }
        }

        internal BaseCommand CreateInstance(CommandKey commandKey, ICommandDataObject commandDataObject)
        {
            var instance = (BaseCommand)Activator.CreateInstance(_command);
            instance.Key = commandKey;
            PropertyInfo prop = _command.GetProperty("DataObject", BindingFlags.Instance | BindingFlags.Public);
            // TODO need to add a some checking is DataObject exist and etc
            prop.SetValue(instance, commandDataObject, null);
            return instance;
        }

    }
}
