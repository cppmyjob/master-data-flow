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
using MasterDataFlow.Network.Packets;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Handlers
{
    // http://stackoverflow.com/questions/658498/how-to-load-assembly-to-appdomain-with-all-references-recursively
    public class ServerGateHandler : BaseHandler, IDisposable
    {
        private readonly AsyncDictionary<BaseKey, CommandRunner> _commandRunnerHubs = new AsyncDictionary<BaseKey, CommandRunner>();

        private BaseKey _clientGateKey;
        private readonly AssemblyLoader _assemblyLoader = new AssemblyLoader();

        public override string[] SupportedActions
        {
            get { return new[] { RemoteExecuteCommandAction.ActionName, SendClientGateKeyAction.ActionName, UploadTypeResponseAction.ActionName}; }
        }


        internal protected override void ConnectHub(IHub hub)
        {
            var commandRunnerHub = hub as CommandRunner;
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

            var workflowKey = (WorkflowKey)BaseKey.DeserializeKey(action.CommandInfo.WorkflowKey);

            if (action.CommandInfo.DataObjectType != null)
            {
                if (!IsTypeExists(workflowKey, action.CommandInfo.DataObjectType))
                {
                    SendUploadTypeCommand(action.CommandInfo.WorkflowKey, action.CommandInfo.DataObjectType, packet);
                    return;
                }
            }

            if (!IsTypeExists(workflowKey, action.CommandInfo.CommandType))
            {
                SendUploadTypeCommand(action.CommandInfo.WorkflowKey, action.CommandInfo.CommandType, packet);
                return;
            }

            BaseKey senderKey = Parent.Key;
            BaseKey recieverKey = allRunners[0].Key;
            object body = new FindContainerAndLaunchCommandAction()
            {
                ExternalDomainCommandInfo = new ExternalDomainCommandInfo
                {
                    CommandKey = action.CommandInfo.CommandKey,
                    CommandType = action.CommandInfo.CommandType,
                    DataObject = action.CommandInfo.DataObject,
                    DataObjectType = action.CommandInfo.DataObjectType,
                    WorkflowKey = workflowKey,
                    AssemblyLoader = _assemblyLoader
                },
            };

            allRunners[0].Send(new Packet(senderKey, recieverKey, body));
        }


        private void SendUploadTypeCommand(string workKlowKey, string typeName, IPacket packet)
        {
            var accumulatorKey = UploadTypeResponseAction.ActionName;
            Parent.Accumulator.Lock(accumulatorKey);
            try
            {
                if (Parent.Accumulator.GetStatus(accumulatorKey) == HubAccumulatorStatus.Free)
                {
                    var uploadAction = new UploadTypeRequestAction
                    {
                        TypeName = typeName,
                        WorkflowKey = workKlowKey,
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

        public bool IsTypeExists(WorkflowKey workflowKey, string typeName)
        {
            return _assemblyLoader.IsTypeExists(workflowKey, typeName);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _assemblyLoader.Dispose();
            }
        }
    }
}
