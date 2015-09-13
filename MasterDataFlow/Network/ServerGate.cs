using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Handlers;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Network.Packets;
using MasterDataFlow.Network.Routing;
using MasterDataFlow.Serialization;

namespace MasterDataFlow.Network
{
    public class ServerGate : Gate, IGateContract, IDisposable
    {
        private readonly IGateCallback _callback;
        private readonly ServerGateHandler _serverGateHandler;

        internal ServerGate()
        {
            _serverGateHandler = new ServerGateHandler();
            RegisterHandler(_serverGateHandler);
        }

        public ServerGate(ServiceKey key, IGateCallback callback) : base(key)
        {
            _callback = callback;
            _serverGateHandler = new ServerGateHandler();
            RegisterHandler(_serverGateHandler);
        }

        public BaseKey ClientGateKey { get; internal set; }

        protected override void ProcessUndeliveredPacket(IPacket packet)
        {
            // TODO need to resend if _callback == null?
            if (_callback != null)
            {
                var bodyTypeName = packet.Body.GetType().AssemblyQualifiedName;
                // TODO need more flexible serialization way
                var body = Serializator.Serialize(packet.Body);
                var remotePacket = new RemotePacket(packet.SenderKey.Key, packet.RecieverKey.Key, bodyTypeName, body);
                _callback.Send(remotePacket); 
            }
        }

        public void Send(RemotePacket remotePacket)
        {
            var bodyType = Type.GetType(remotePacket.TypeName);
            var body = Serializator.Deserialize(bodyType, (string)remotePacket.Body);
            var senderKey = BaseKey.DeserializeKey(remotePacket.SenderKey);
            var recieverKey = BaseKey.DeserializeKey(remotePacket.RecieverKey);
            var packet = new Packet(senderKey, recieverKey, body);
            Send(packet);
        }

        protected override void RelayRequest(string requestId, BaseKey destinationPoint)
        {
            base.RelayRequest(requestId, destinationPoint);
            if (ClientGateKey != null)
            {
                var requestPacket = new Packet(Key, ClientGateKey, new RouteRequest(requestId, destinationPoint));
                Send(requestPacket);
            }
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
                _serverGateHandler.Dispose();
            }
        }
    }
}
