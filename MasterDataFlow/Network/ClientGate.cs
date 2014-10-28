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
        private readonly IClientContext _context;
        private CommandRunnerHub _runner;
        private bool _isServerGateInitializated = false;
        private object _syncObject = new object();

        public ClientGate(IClientContext remoteHostContract)
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
            // TODO it needs another way for passing ClientGateKey to server side
            lock (_syncObject)
            {
                if (!_isServerGateInitializated)
                {
                    InitializateServerGate();
                    _isServerGateInitializated = true;
                }
            }

            var bodyTypeName = packet.Body.GetType().AssemblyQualifiedName;
            // TODO need more flexible serialization way
            var body = Serialization.Serializator.Serialize(packet.Body);
            var remotePacket = new RemotePacket(packet.SenderKey.Key, packet.RecieverKey.Key, bodyTypeName, body);
            _context.Contract.Send(remotePacket);

        }

        private void InitializateServerGate()
        {
            return;
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
