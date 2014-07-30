using System;
using System.Collections.Generic;
using System.Linq;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Utils;

namespace MasterDataFlow
{
    public delegate void OnNextCommand(BaseCommand command);

    public delegate void OnCommandError(CommandInfo info);

    public delegate void OnMessageRecieved(Guid loopId, EventLoopCommandStatus status, ILoopCommandMessage message);

    public class CommandWorkflow : ICommandWorkflow
    {
        private readonly IList<CommandDefinition> _definitions = new List<CommandDefinition>();
        private readonly Guid _id;
        private readonly CommandRunner _runner;

        public event OnMessageRecieved MessageRecieved;

        internal CommandWorkflow(Guid id, CommandRunner runner)
        {
            _id = id;
            _runner = runner;
        }

        public CommandWorkflow(CommandRunner runner)
        {
            _id = Guid.NewGuid();
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
            Type commandType = typeof(TCommand);
            return Find(commandType);
        }

        public void Register(CommandDefinition definition)
        {
            _definitions.Add(definition);
        }

        public Guid Start<TCommand>(ICommandDataObject commandDataObject)
            where TCommand : ICommand<ICommandDataObject>
        {
            Type commandType = typeof(TCommand);

            CommandDefinition commandDefinition = Find(commandType);
            // TODO check if commandDefinition was found
            return _runner.Run(this, commandDefinition, commandDataObject);
        }

        private void EventLoopCallback(Guid loopId, EventLoopCommandStatus status, ILoopCommandMessage message)
        {
            if (MessageRecieved != null)
            {
                MessageRecieved(loopId, status, message);
            }
        }

        private CommandDefinition Find(Type commandType)
        {
            CommandDefinition result = _definitions.FirstOrDefault(t => t.Command == commandType);
            return result;
        }

        void ICommandWorkflow.EventLoopCallback(Guid loopId, EventLoopCommandStatus status, ILoopCommandMessage message)
        {
            EventLoopCallback(loopId, status, message);
        }
    }
}