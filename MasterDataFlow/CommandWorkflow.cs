using System;
using System.Collections.Generic;
using System.Linq;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Utils;

namespace MasterDataFlow
{
    public delegate void OnNextCommand(BaseCommand command);

    public delegate void OnCommandError(CommandInfo info);

    public delegate void OnMessageRecieved(Guid loopId, EventLoopCommandStatus status, ILoopCommandMessage message);

    public class CommandWorkflow : ICommandWorkflow
    {
        private readonly IList<CommandDefinition> _definitions = new List<CommandDefinition>();
        private readonly WorkflowKey _key;
        private readonly CommandRunner _runner;

        public event OnMessageRecieved MessageRecieved;

        internal CommandWorkflow(WorkflowKey key, CommandRunner runner)
        {
            _key = key;
            _runner = runner;
        }

        public CommandWorkflow(CommandRunner runner)
        {
            _key = new WorkflowKey();
            _runner = runner;
        }

        internal IList<CommandDefinition> Definitions
        {
            get { return _definitions; }
        }

        public WorkflowKey Key
        {
            get { return _key; }
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
            var commandKey = new CommandKey();
            CommandDefinition commandDefinition = Find(commandType);
            // TODO check if commandDefinition was found
            return _runner.Run(this, commandKey, commandDefinition, commandDataObject);
        }

        public void Subscribe(BaseKey key)
        {

        }

        public void Unsubscribe(BaseKey key)
        {
            throw new NotImplementedException();
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