using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Actions;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Network
{
    public class ClientGate : Gate
    {
        private readonly IRemoteClientContext _context;
        private CommandRunnerHub _runner;

        public ClientGate(IRemoteClientContext remoteHostContract)
        {
            _context = remoteHostContract;
        }

        public BaseKey ServerGateKey
        {
            get { return _context.ServerGateKey; } 
        }

        public override void AcceptHub(IHub hub)
        {
            // TODO Check CommandRunnerHub
            _runner = (CommandRunnerHub)hub;
        }

        protected override void ProccessPacket(IPacket packet)
        {

        }

        protected override void ProcessUndeliveredPacket(IPacket packet)
        {
            string bodyTypeName = packet.Body.GetType().AssemblyQualifiedName;
            // TODO need more flexible serialization way
            var body = Serialization.Serializator.Serialize(packet.Body);
            var remotePacket = new RemotePacket(packet.SenderKey, packet.RecieverKey, bodyTypeName, body);
            _context.Contract.Send(remotePacket);
        }

    }
}
