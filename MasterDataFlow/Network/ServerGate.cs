using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Handlers;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Serialization;

namespace MasterDataFlow.Network
{
    public class ServerGate : Gate, IGateContract
    {
        private IGateCallback _callback;

        public ServerGate()
        {
            RegisterHandler(new ServerGateHandler());
        }

        public ServerGate(ServiceKey key, IGateCallback callback) : base(key)
        {
            _callback = callback;
            RegisterHandler(new ServerGateHandler());
        }

        public BaseKey ClientGateKey { get; internal set; }

        protected override void ProcessUndeliveredPacket(IPacket packet)
        {
            if (packet.RecieverKey == ClientGateKey)
            {
                if (_callback != null)
                {
                    var bodyTypeName = packet.Body.GetType().AssemblyQualifiedName;
                    // TODO need more flexible serialization way
                    var body = Serialization.Serializator.Serialize(packet.Body);
                    var remotePacket = new RemotePacket(packet.SenderKey.Key, packet.RecieverKey.Key, bodyTypeName, body);
                    _callback.Send(remotePacket); 
                }
            }
        }

        public void UploadAssembly(string typeName, byte[] data)
        {
            throw new NotImplementedException();
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
    }
}
