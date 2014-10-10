using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Actions;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Network
{
    public class CommandWorkflowHub : Hub
    {
        private readonly IList<CommandDefinition> _definitions = new List<CommandDefinition>();
        private readonly WorkflowKey _key = new WorkflowKey();
        private CommandRunnerHub _runner;

        //public event OnMessageRecieved MessageRecieved;

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

        protected override void ProccessPacket(Interfaces.Network.IPacket packet)
        {
            throw new NotImplementedException();
        }

    }
}
