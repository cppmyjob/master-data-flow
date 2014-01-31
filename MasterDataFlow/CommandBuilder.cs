using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow
{
    public class CommandBuilder
    {
        private CommandDefinition _commandDefinition;

        private CommandBuilder(Type commandType)
        {
            _commandDefinition = new CommandDefinition(commandType);
        }

        public static CommandBuilder Build<TCommand>()
            where TCommand : ICommand<ICommandDataObject>
        {
            return new CommandBuilder(typeof(TCommand));
        }

        public CommandDefinition Complete()
        {
            return _commandDefinition;
        }
    }
}
