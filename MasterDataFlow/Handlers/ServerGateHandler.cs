using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Actions;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Handlers
{
    public class ServerGateHandler : BaseHandler
    {
        private readonly AsyncDictionary<BaseKey, CommandRunnerHub> _commandRunnerHubs = new AsyncDictionary<BaseKey, CommandRunnerHub>();

        public override string[] SupportedActions
        {
            get { return new [] {RemoteExecuteCommandAction.ActionName}; }
        }


        internal protected override void ConnectHub(IHub hub)
        {
            if (hub is CommandRunnerHub)
            {
                _commandRunnerHubs.AddItem(hub.Key, (CommandRunnerHub)hub);
            }
        }

        public override void Execute(string actionName, IPacket packet)
        {
            switch (actionName)
            {
                case RemoteExecuteCommandAction.ActionName:
                    RemoteExecuteCommandActionProcess((RemoteExecuteCommandAction)packet.Body);
                    break;
                default:
                    // TODO Send Error Exception
                    break;
            }
        }

        private void RemoteExecuteCommandActionProcess(RemoteExecuteCommandAction action)
        {
            var allRunners = _commandRunnerHubs.GetItems();
            if (allRunners.Count == 0)
                // TODO Send error message
                return;
            ICommandDataObject dataObject = null;
            if (action.CommandInfo.DataObjectType != null)
            {
                var dataObjectType = Type.GetType(action.CommandInfo.DataObjectType);
                dataObject = (ICommandDataObject) Serialization.Serializator.Deserialize(dataObjectType, action.CommandInfo.DataObject);
            }

            var commandType = Type.GetType(action.CommandInfo.CommandType);
            var commandDefinition = new CommandDefinition(commandType);

            BaseKey senderKey = Parent.Key;
            BaseKey recieverKey = allRunners[0].Key;
            object body = new FindContainerAndLaunchCommandAction()
            {
                CommandInfo = new CommandInfo()
                {
                    CommandKey = BaseKey.DeserializeKey<CommandKey>(action.CommandInfo.CommandKey),
                    WorkflowKey = BaseKey.DeserializeKey<WorkflowKey>(action.CommandInfo.WorkflowKey),
                    CommandDefinition = commandDefinition,
                    CommandDataObject = dataObject
                }
            };

            allRunners[0].Send(new Packet(senderKey, recieverKey, body));

        }
    }
}
