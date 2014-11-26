using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using MasterDataFlow.Actions;
using MasterDataFlow.Actions.ClientGateKey;
using MasterDataFlow.Actions.UploadType;
using MasterDataFlow.Assemblies;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Handlers
{
    // http://stackoverflow.com/questions/658498/how-to-load-assembly-to-appdomain-with-all-references-recursively
    public class ServerGateHandler : BaseHandler, IInstanceFactory
    {
        private readonly AsyncDictionary<BaseKey, CommandRunnerHub> _commandRunnerHubs = new AsyncDictionary<BaseKey, CommandRunnerHub>();

        private BaseKey _clientGateKey;
        private readonly AssemblyLoader _assemblyLoader = new AssemblyLoader();

        public override string[] SupportedActions
        {
            get { return new[] { RemoteExecuteCommandAction.ActionName, SendClientGateKeyAction.ActionName, UploadTypeResponseAction.ActionName}; }
        }


        internal protected override void ConnectHub(IHub hub)
        {
            var commandRunnerHub = hub as CommandRunnerHub;
            if (commandRunnerHub != null)
            {
                _commandRunnerHubs.AddItem(hub.Key, commandRunnerHub);
            }
        }

        public override void Execute(string actionName, IPacket packet)
        {
            switch (actionName)
            {
                case RemoteExecuteCommandAction.ActionName:
                    ProcessRemoteExecuteCommandAction((RemoteExecuteCommandAction)packet.Body, packet);
                    break;
                case SendClientGateKeyAction.ActionName:
                    ProcessSendClientGateKeyAction((SendClientGateKeyAction) packet.Body);
                    break;
                case UploadTypeResponseAction.ActionName:
                    ProcessUploadTypeResponseAction((UploadTypeResponseAction) packet.Body);
                    break;
                default:
                    // TODO Send Error Exception
                    break;
            }
        }

        private void ProcessUploadTypeResponseAction(UploadTypeResponseAction body)
        {
            var accumulatorKey = UploadTypeResponseAction.ActionName;
            Parent.Accumulator.Lock(accumulatorKey);
            try
            {
                var workflowKey = BaseKey.DeserializeKey(body.WorkflowKey);
                _assemblyLoader.Load(workflowKey, body.AssemblyName, body.AssemblyData);
                var packets = Parent.Accumulator.Extract(accumulatorKey);
                if (packets != null)
                {
                    foreach (var packet in packets)
                    {
                        Parent.Send(packet);
                    }
                }
            }
            finally
            {
                Parent.Accumulator.UnLock(accumulatorKey);
            }
        }

        private void ProcessSendClientGateKeyAction(SendClientGateKeyAction action)
        {
             _clientGateKey = BaseKey.DeserializeKey(action.ClientGateKey);
             ((ServerGate) Parent).ClientGateKey = _clientGateKey;
            var responseAction = new ClientGateKeyRecievedAction();
            Parent.Send(new Packet(Parent.Key, _clientGateKey, responseAction));
        }

        private void ProcessRemoteExecuteCommandAction(RemoteExecuteCommandAction action, IPacket packet)
        {
            var allRunners = _commandRunnerHubs.GetItems();
            if (allRunners.Count == 0)
                // TODO Send error message
                return;

            var workflowKey = (WorkflowKey) BaseKey.DeserializeKey(action.CommandInfo.WorkflowKey);

            ICommandDataObject dataObject = null;
            if (action.CommandInfo.DataObjectType != null)
            {
                Type dataObjectType = GetType(workflowKey, action.CommandInfo.DataObjectType);
                if (dataObjectType == null)
                {
                    SendUploadTypeCommand(action, packet);
                    return;
                }
                dataObject = (ICommandDataObject)Serialization.Serializator.DeserializeDataObject(dataObjectType, action.CommandInfo.DataObject, workflowKey, this);
            }

            Type commandType = GetType(workflowKey, action.CommandInfo.CommandType);
            if (commandType == null)
            {
                SendUploadTypeCommand(action, packet);
                return;
            }

            //var commandDefinition = new CommandDefinition(commandType);

            BaseKey senderKey = Parent.Key;
            BaseKey recieverKey = allRunners[0].Key;
            object body = new FindContainerAndLaunchCommandAction()
            {
                CommandInfo = new CommandInfo()
                {
                    CommandKey = (CommandKey)BaseKey.DeserializeKey(action.CommandInfo.CommandKey),
                    WorkflowKey = workflowKey,
                    CommandType = commandType,
                    CommandDataObject = dataObject,
                    InstanceFactory = this
                }
            };

            allRunners[0].Send(new Packet(senderKey, recieverKey, body));
        }

        private void SendUploadTypeCommand(RemoteExecuteCommandAction action, IPacket packet)
        {
            var accumulatorKey = UploadTypeResponseAction.ActionName;
            Parent.Accumulator.Lock(accumulatorKey);
            try
            {
                if (Parent.Accumulator.GetStatus(accumulatorKey) == HubAccumulatorStatus.Free)
                {
                    var uploadAction = new UploadTypeRequestAction
                    {
                        TypeName = action.CommandInfo.DataObjectType,
                        WorkflowKey = action.CommandInfo.WorkflowKey,
                    };
                    Parent.Send(new Packet(Parent.Key, _clientGateKey, uploadAction));
                    Parent.Accumulator.SetBusyStatus(accumulatorKey);
                }
                // TODO it needs to use accumulator key with UploadType
                Parent.Accumulator.Add(accumulatorKey, packet);
            }
            finally
            {
                Parent.Accumulator.UnLock(accumulatorKey);
            }
        }

        public BaseCommand CreateCommandInstance(WorkflowKey workflowKey, CommandKey commandKey, Type type, ICommandDataObject commandDataObject)
        {
            var instance = (BaseCommand)_assemblyLoader.CreateInstance(workflowKey, type);
            instance.Key = commandKey;
            PropertyInfo prop = type.GetProperty("DataObject", BindingFlags.Instance | BindingFlags.Public);
            // TODO need to add a some checking is DataObject exist and etc
            prop.SetValue(instance, commandDataObject, null);
            return instance;
        }

        public Type GetType(WorkflowKey workflowKey, string typeName)
        {
            return _assemblyLoader.GetLoadedType(workflowKey, typeName);
        }
    }
}
