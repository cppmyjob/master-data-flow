using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Exceptions;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow
{
    public delegate void OnNextCommand(BaseCommand command);
    public delegate void OnCommandError(CommandInfo info);

    public class CommandDomain
    {
        private readonly IList<CommandDefinition> _definitions = new List<CommandDefinition>();

        internal IList<CommandDefinition> Definitions
        {
            get { return _definitions; }
        }

        public CommandDefinition Find<TCommand>()
            where TCommand : ICommand<ICommandDataObject>
        {
            var commandType = typeof (TCommand);
            var result = _definitions.FirstOrDefault(t => t.Command == commandType);
            return result;
        }

        public void Register(CommandDefinition definition)
        {
            _definitions.Add(definition);
        }

    }
}
