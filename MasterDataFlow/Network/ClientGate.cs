using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MasterDataFlow.Actions;
using MasterDataFlow.Actions.ClientGateKey;
using MasterDataFlow.Actions.UploadType;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Network.Packets;
using MasterDataFlow.Serialization;

namespace MasterDataFlow.Network
{
    public class ClientGate : Gate
    {
        private readonly IClientContext _context;
        private CommandRunner _runner;

        public ClientGate(IClientContext remoteHostContract)
        {
            _context = remoteHostContract;
            _context.GateCallbackPacketRecieved += OnGateCallbackPacketRecievedHandler;
            if (_context.IsNeedSendKey)
            {
                Accumulator.Lock(ClientGateKeyRecievedAction.ActionName);
                try
                {
                    SendClientKey();
                    Accumulator.SetBusyStatus(ClientGateKeyRecievedAction.ActionName);
                }
                finally
                {
                    Accumulator.UnLock(ClientGateKeyRecievedAction.ActionName);
                }
            }
        }

        public BaseKey ServerGateKey
        {
            get { return _context.ServerGateKey; } 
        }

        public override void AcceptHub(IHub hub)
        {
            base.AcceptHub(hub);
            // TODO Check CommandRunnerHub
            _runner = (CommandRunner)hub;
        }

        protected override void ProccessPacket(IPacket packet)
        {
            if (packet.Body == null)
                // TODO Exception?
                return;
            var action = packet.Body as BaseAction;
            if (action == null)
                // TODO Exception?
                return;
            switch (action.Name)
            {
                case ClientGateKeyRecievedAction.ActionName:
                    ProcessClientGateKeyRecievedAction();
                    break;
                case UploadTypeRequestAction.ActionName:
                    ProcessUploadTypeAction(action as UploadTypeRequestAction);
                    break;
                default:
                    // TODO Exception?
                    break;
            }
        }

        protected override void RelayRequest(string requestId, BaseKey destinationPoint)
        {
            base.RelayRequest(requestId, destinationPoint);
        }

        private void OnGateCallbackPacketRecievedHandler(RemotePacket remotePacket)
        {
            var bodyType = Type.GetType(remotePacket.TypeName);
            var body = Serializator.Deserialize(bodyType, remotePacket.Body);
            var senderKey = BaseKey.DeserializeKey(remotePacket.SenderKey);
            var recieverKey = BaseKey.DeserializeKey(remotePacket.RecieverKey);
            var packet = new Packet(senderKey, recieverKey, body);

            if (body is CommandMessage)
                SendPacketViaRouting(packet);
            else
                Send(packet);
        }

        private void ProcessUploadTypeAction(UploadTypeRequestAction action)
        {
            var actionType = Type.GetType(action.TypeName);
            string path = actionType.Assembly.Location;
            var assemblyFilename = Path.GetFileName(path);
            using (var stream = File.OpenRead(path))
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                var responseAction = new UploadTypeResponseAction
                {
                    TypeName = action.TypeName,
                    AssemblyData = buffer,
                    AssemblyName = assemblyFilename,
                    WorkflowKey = action.WorkflowKey
                };
                Send(new Packet(Key, ServerGateKey, responseAction));
            }
        }

        private void ProcessClientGateKeyRecievedAction()
        {
            Accumulator.Lock(ClientGateKeyRecievedAction.ActionName);
            try
            {
                var packets = Accumulator.Extract(ClientGateKeyRecievedAction.ActionName);
                if (packets != null)
                {
                    foreach (var packet in packets)
                    {
                        Send(packet);
                    }
                }
            }
            finally
            {
                Accumulator.UnLock(ClientGateKeyRecievedAction.ActionName);
            }
        }

        protected override void ProcessUndeliveredPacket(IPacket packet)
        {
            Accumulator.Lock(ClientGateKeyRecievedAction.ActionName);
            try
            {
                if (Accumulator.GetStatus(ClientGateKeyRecievedAction.ActionName) == HubAccumulatorStatus.Busy)
                {
                    Accumulator.Add(ClientGateKeyRecievedAction.ActionName, packet);
                    return;
                }
            }
            finally
            {
                Accumulator.UnLock(ClientGateKeyRecievedAction.ActionName);
            }

            // TODO Merge with SendClientKey begin from var bodyTypeName =.....
            var bodyTypeName = packet.Body.GetType().AssemblyQualifiedName;
            // TODO need more flexible serialization way
            var body = Serialization.Serializator.Serialize(packet.Body);
            var remotePacket = new RemotePacket(packet.SenderKey.Key, packet.RecieverKey.Key, bodyTypeName, body);
            _context.Contract.Send(remotePacket);

        }

        private void SendClientKey()
        {
            var sendClientGateKeyAction = new SendClientGateKeyAction()
            {
                ClientGateKey = Key.Key
            };
            var bodyTypeName = sendClientGateKeyAction.GetType().AssemblyQualifiedName;
            // TODO need more flexible serialization way
            var body = Serialization.Serializator.Serialize(sendClientGateKeyAction);
            var remotePacket = new RemotePacket(Key.Key, ServerGateKey.Key, bodyTypeName, body);
            _context.Contract.Send(remotePacket);
        }
    }
}
