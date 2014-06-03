using System;
using System.Collections.Generic;
using System.Linq;
using MasterDataFlow._20140530.Interfaces;

namespace MasterDataFlow._20140530
{
    public delegate void OnNextCommand(BaseCommand command);
    public delegate void OnCommandError(CommandInfo info);

    public class CommandDomain
    {
        private readonly IList<CommandDefinition> _definitions = new List<CommandDefinition>();
        private readonly Guid _id = Guid.NewGuid();

        internal IList<CommandDefinition> Definitions
        {
            get { return _definitions; }
        }

        public Guid Id
        {
            get { return _id; }
        }

        public CommandDefinition Find<TCommand>()
            where TCommand : ICommand<ICommandDataObject>
        {
            var commandType = typeof (TCommand);
            return Find(commandType);
        }

        internal CommandDefinition Find(Type commandType)
        {
            var result = _definitions.FirstOrDefault(t => t.Command == commandType);
            return result;
        }

        public void Register(CommandDefinition definition)
        {
            _definitions.Add(definition);
        }

    }
}
