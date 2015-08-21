using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;
using MasterDataFlow.Network.Packets;

namespace MasterDataFlow.Intelligence.Tests
{
    public class RemoteEvironment
    {
        public class RemoteClientContextMock : IClientContext, IGateCallback
        {
            private IGateContract _gateContract;
            private readonly BaseKey _serverGateKey;

            public RemoteClientContextMock(BaseKey serverGateKey)
            {
                _serverGateKey = serverGateKey;
            }

            public IGateContract Contract
            {
                get { return _gateContract; }
            }

            public BaseKey ServerGateKey
            {
                get { return _serverGateKey; }
            }

            public bool IsNeedSendKey
            {
                get { return true; }
            }

            public void Dispose()
            {

            }

            public void SetContract(IGateContract contract)
            {
                _gateContract = contract;
            }

            public event GateCallbackPacketRecievedHandler GateCallbackPacketRecieved;

            public void Send(RemotePacket packet)
            {
                if (GateCallbackPacketRecieved != null)
                {
                    GateCallbackPacketRecieved(packet);
                }
            }
        }


        public CommandWorkflow PrepeareSingleRemoteContainer()
        {
            var serverGateKey = new ServiceKey();
            var remoteClientContext = new RemoteClientContextMock(serverGateKey);

            var remoteCommandRunner = new CommandRunner();
            var serverGate = new ServerGate(serverGateKey, remoteClientContext);
            serverGate.ConnectHub(remoteCommandRunner);

            var remoteContainer = new SimpleContainer();
            remoteCommandRunner.ConnectHub(remoteContainer);

            var runner = new CommandRunner();

            remoteClientContext.SetContract(serverGate);
            var clientGate = new ClientGate(remoteClientContext);
            runner.ConnectHub(clientGate);

            var сommandWorkflow = new CommandWorkflow();
            runner.ConnectHub(сommandWorkflow);

            return сommandWorkflow;
        }
    }
}
