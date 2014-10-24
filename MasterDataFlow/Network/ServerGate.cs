using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Handlers;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Messages;
using MasterDataFlow.Serialization;

namespace MasterDataFlow.Network
{
    public class ServerGate : Gate, IRemoteHostContract
    {
        public ServerGate()
        {
            RegisterHandler(new ServerGateHandler());
        }

        public void UploadAssembly(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void Send(RemotePacket remotePacket)
        {
            var bodyType = Type.GetType(remotePacket.TypeName);
            var body = Serializator.Deserialize(bodyType, (string)remotePacket.Body);
            var packet = new Packet(remotePacket.SenderKey, remotePacket.RecieverKey, body);
            Send(packet);
        }
    }
}
