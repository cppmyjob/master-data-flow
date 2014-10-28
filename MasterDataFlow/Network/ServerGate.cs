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
        public ServerGate()
        {
            RegisterHandler(new ServerGateHandler());
        }

        public ServerGate(ServiceKey key) : base(key)
        {
            RegisterHandler(new ServerGateHandler());
        }

        public BaseKey ClientGateKey { get; internal set; }

        protected override void ProcessUndeliveredPacket(IPacket packet)
        {
            if (packet.RecieverKey == ClientGateKey)
            {
                
            }
        }

        public void UploadAssembly(byte[] data)
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
