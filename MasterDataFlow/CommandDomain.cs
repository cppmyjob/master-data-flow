using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Exceptions;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow
{
    public delegate void OnNextCommand(BaseCommand command);
    public delegate void OnCommandError(CommandInfo info);

    public class CommandDomain
    {
        private readonly IList<CommandDefinition> _definitions = new List<CommandDefinition>();
        private readonly Guid _id = Guid.NewGuid();
        private readonly CommandRunner _runner;

        public CommandDomain(CommandRunner runner)
        {
            _runner = runner;
        }

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

        public Guid Start<TCommand>(ICommandDataObject commandDataObject, EventLoopCallback callback = null)
            where TCommand : ICommand<ICommandDataObject>
        {
            var commandType = typeof(TCommand);

            var commandDefinition = Find(commandType);
            // TODO check if commandDefinition was found
            return _runner.Run(this, commandDefinition, commandDataObject, callback);
        }

    }
}
