using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    public class CommandWorkflowHub : EventLoopHub, IInstanceFactory
    {
        private readonly WorkflowKey _key;
        private CommandRunnerHub _runner;

        public CommandWorkflowHub()
        {
            _key = new WorkflowKey();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event OnMessageRecieved MessageRecieved;

        public override BaseKey Key
        {
            get { return _key; }
        }

        public override void AcceptHub(IHub hub)
        {
            base.AcceptHub(hub);
            // TODO Check CommandRunnerHub
            _runner = (CommandRunnerHub)hub;
        }

        public CommandKey Start<TCommand>(ICommandDataObject commandDataObject)
            where TCommand : ICommand<ICommandDataObject>
        {
            Type commandType = typeof(TCommand);
            var commandKey = new CommandKey();
            //CommandDefinition commandDefinition = Find(commandType);
            //// TODO check if commandDefinition was found
            //if (commandDefinition == null)
            //    throw new MasterDataFlowException("Can't find a command definition for " + commandType.AssemblyQualifiedName);

            BaseKey senderKey = _key;
            BaseKey recieverKey = _runner.Key;
            object body = new FindContainerAndLaunchCommandAction()
            {
                CommandInfo = new CommandInfo()
                {
                    CommandKey = commandKey,
                    WorkflowKey = _key,
                    CommandType = commandType,
                    CommandDataObject = commandDataObject,
                    InstanceFactory = this,
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

        public BaseCommand CreateCommandInstance(WorkflowKey workflowKey, CommandKey commandKey, Type type, ICommandDataObject commandDataObject)
        {
            var instance = (BaseCommand)Activator.CreateInstance(type);
            instance.Key = commandKey;
            PropertyInfo prop = type.GetProperty("DataObject", BindingFlags.Instance | BindingFlags.Public);
            // TODO need to add a some checking is DataObject exist and etc
            prop.SetValue(instance, commandDataObject, null);
            return instance;
        }

        public Type GetType(WorkflowKey workflowKey, string typeName)
        {
            return Type.GetType(typeName);
        }
    }
}
