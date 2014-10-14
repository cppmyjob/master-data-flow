using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Actions;
using MasterDataFlow.Exceptions;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;

namespace MasterDataFlow.Network
{
    public delegate void OnMessageRecieved(BaseKey senderKey,BaseMessage message);

    public class CommandWorkflowHub : EventLoopHub
    {
        private readonly IList<CommandDefinition> _definitions = new List<CommandDefinition>();
        private readonly WorkflowKey _key;
        private CommandRunnerHub _runner;

        public CommandWorkflowHub()
        {
            _key = new WorkflowKey();
        }

        public event OnMessageRecieved MessageRecieved;

        internal IList<CommandDefinition> Definitions
        {
            get { return _definitions; }
        }

        public override BaseKey Key
        {
            get { return _key; }
        }

        public override void AcceptHub(IHub hub)
        {
            _runner = (CommandRunnerHub)hub;
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

        public CommandKey Start<TCommand>(ICommandDataObject commandDataObject)
            where TCommand : ICommand<ICommandDataObject>
        {
            Type commandType = typeof(TCommand);
            var commandKey = new CommandKey();
            CommandDefinition commandDefinition = Find(commandType);
            // TODO check if commandDefinition was found
            if (commandDefinition == null)
                throw new MasterDataFlowException("Can't find a command definition for " + commandType.AssemblyQualifiedName);

            BaseKey senderKey = _key;
            BaseKey recieverKey = _runner.Key;
            object body = new FindContainerAndLaunchCommandAction()
            {
                CommandInfo = new CommandInfo()
                {
                    CommandKey = commandKey,
                    WorkflowKey = _key,
                    CommandDefinition = commandDefinition,
                    CommandDataObject = commandDataObject
                }
            };
            _runner.Send(new Packet(senderKey, recieverKey, body));
            return commandKey;
        }

        public void Subscribe(BaseKey key)
        {

        }

        public void Unsubscribe(BaseKey key)
        {
            throw new NotImplementedException();
        }

        private CommandDefinition Find(Type commandType)
        {
            CommandDefinition result = _definitions.FirstOrDefault(t => t.Command == commandType);
            return result;
        }

        protected override void ProccessPacket(IPacket packet)
        {
            var message = packet.Body as BaseMessage;
            if (message != null)
            {
                if (MessageRecieved != null)
                {
                    MessageRecieved(packet.SenderKey, message);
                }
            }
        }

    }
}
