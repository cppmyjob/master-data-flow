using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        private BaseKey _clientGateKey;

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
                    RemoteExecuteCommandActionProcess((RemoteExecuteCommandAction)packet.Body, packet);
                    break;
                case SendClientGateKeyAction.ActionName:
                    SendClientGateKeyActionProcess((SendClientGateKeyAction) packet.Body);
                    break;
                default:
                    // TODO Send Error Exception
                    break;
            }
        }

        private void SendClientGateKeyActionProcess(SendClientGateKeyAction action)
        {
            _clientGateKey = BaseKey.DeserializeKey(action.ClientGateKey);
            ((ServerGate) Parent).ClientGateKey = _clientGateKey;
        }

        private void RemoteExecuteCommandActionProcess(RemoteExecuteCommandAction action, IPacket packet)
        {
            var allRunners = _commandRunnerHubs.GetItems();
            if (allRunners.Count == 0)
                // TODO Send error message
                return;
            ICommandDataObject dataObject = null;
            if (action.CommandInfo.DataObjectType != null)
            {
                var dataObjectType = Type.GetType(action.CommandInfo.DataObjectType);
                if (dataObjectType == null)
                {
                    SendUploadTypeCommand(action, packet);
                    return;
                }
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
                    CommandKey = (CommandKey)BaseKey.DeserializeKey(action.CommandInfo.CommandKey),
                    WorkflowKey = (WorkflowKey)BaseKey.DeserializeKey(action.CommandInfo.WorkflowKey),
                    CommandDefinition = commandDefinition,
                    CommandDataObject = dataObject
                }
            };

            allRunners[0].Send(new Packet(senderKey, recieverKey, body));

        }

        private void SendUploadTypeCommand(RemoteExecuteCommandAction action, IPacket packet)
        {
            var uploadAction = new UploadTypeAction();
            uploadAction.UploadType = action.CommandInfo.DataObjectType;
            Parent.Send(new Packet(Parent.Key, _clientGateKey, uploadAction));

            Thread.Sleep(100);
            Parent.Send(packet);
        }
    }
}
