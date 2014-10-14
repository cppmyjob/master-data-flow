﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Actions;
using MasterDataFlow.Exceptions;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Handlers
{
    public class CommandRunnerHandler : BaseHandler
    {
        private readonly AsyncDictionary<BaseKey, SimpleContainerHub> _simpleContainers = new AsyncDictionary<BaseKey, SimpleContainerHub>();
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
            if (hub is SimpleContainerHub)
            {
                _simpleContainers.AddItem(hub.Key, (SimpleContainerHub) hub);
            }
            if (hub is ClientGate)
            {
                _clientGateContainers.AddItem(hub.Key, (ClientGate)hub);
            }
        }

        private void ProcessFindContainerAndLaunchCommandAction(FindContainerAndLaunchCommandAction action)
        {
            List<SimpleContainerHub> allContainers = _simpleContainers.GetItems();
            if (allContainers.Count == 0)
            {
                ProcessRemoteExecuteCommandAction(action);
                return;
            }
            // TODO Select an one from multi
            var recieverKey = allContainers[0].Key;
            BaseKey senderKey = Parent.Key;
            object body = new LocalExecuteCommandAction()
            {
                CommandInfo = action.CommandInfo
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
                CommandType = action.CommandInfo.CommandDefinition.Command.AssemblyQualifiedName,
                DataObject = Serialization.Serializator.Serialize(action.CommandInfo.CommandDataObject),
                DataObjectType = action.CommandInfo.CommandDataObject.GetType().AssemblyQualifiedName,
                WorkflowKey = action.CommandInfo.WorkflowKey.Key,
                CommandKey = action.CommandInfo.CommandKey.Key
            };

            var recieverKey = allGates[0].ServerGateKey;
            BaseKey senderKey = Parent.Key;
            var remoteAction = new RemoteExecuteCommandAction { CommandInfo = info };
            var packet = new Packet(senderKey, recieverKey, remoteAction);
            allGates[0].Send(packet);
        }


    }
}