﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Actions;
using MasterDataFlow.Exceptions;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;
using MasterDataFlow.Network.Packets;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Handlers
{
    public class CommandRunnerHandler : BaseHandler
    {
        private readonly AsyncDictionary<BaseKey, SimpleContainer> _simpleContainers = new AsyncDictionary<BaseKey, SimpleContainer>();
        private readonly AsyncDictionary<BaseKey, ClientGate> _clientGateContainers = new AsyncDictionary<BaseKey, ClientGate>();

        public override string[] SupportedActions
        {
            get { return new[] { FindContainerAndLaunchCommandAction.ActionName }; }
        }

        public override void Execute(string actionName, IPacket packet)
        {
            switch (actionName)
            {
                case FindContainerAndLaunchCommandAction.ActionName:
                    ProcessFindContainerAndLaunchCommandAction((FindContainerAndLaunchCommandAction) packet.Body);
                    break;
                default:
                    // TODO Send Error Exception
                    break;
            }
        }

        internal protected override void ConnectHub(IHub hub)
        {
            var simpleContainerHub = hub as SimpleContainer;
            if (simpleContainerHub != null)
            {
                _simpleContainers.AddItem(hub.Key, simpleContainerHub);
            }
            var clientGate = hub as ClientGate;
            if (clientGate != null)
            {
                _clientGateContainers.AddItem(hub.Key, clientGate);
            }
        }

        private void ProcessFindContainerAndLaunchCommandAction(FindContainerAndLaunchCommandAction action)
        {
            var allContainers = _simpleContainers.GetItems();
            if (allContainers.Count == 0)
            {
                ProcessRemoteExecuteCommandAction(action);
                return;
            }
            // TODO Select an one from multi
            var recieverKey = allContainers[0].Key;
            var senderKey = Parent.Key;
            object body = new LocalExecuteCommandAction()
            {
                CommandInfo = action.LocalDomainCommandInfo,
                ExternalDomainCommandInfo = action.ExternalDomainCommandInfo
            };
            allContainers[0].Send(new Packet(senderKey, recieverKey, body));
        }

        private void ProcessRemoteExecuteCommandAction(FindContainerAndLaunchCommandAction action)
        {
            List<ClientGate> allGates = _clientGateContainers.GetItems();
            if (allGates.Count == 0)
                // TODO Send error message
                return;
            // TODO Select an one from multi
            var info = new RemoteExecuteCommandAction.Info
            {
                CommandType = action.LocalDomainCommandInfo.CommandType.AssemblyQualifiedName,
                DataObject = action.LocalDomainCommandInfo.CommandDataObject != null ? Serialization.Serializator.Serialize(action.LocalDomainCommandInfo.CommandDataObject) : null,
                DataObjectType = action.LocalDomainCommandInfo.CommandDataObject != null ? action.LocalDomainCommandInfo.CommandDataObject.GetType().AssemblyQualifiedName : null,
                WorkflowKey = action.LocalDomainCommandInfo.WorkflowKey.Key,
                CommandKey = action.LocalDomainCommandInfo.CommandKey.Key
            };

            var recieverKey = allGates[0].ServerGateKey;
            BaseKey senderKey = Parent.Key;
            var remoteAction = new RemoteExecuteCommandAction { CommandInfo = info };
            var packet = new Packet(senderKey, recieverKey, remoteAction);
            allGates[0].Send(packet);
        }


    }
}
