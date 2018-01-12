using System;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;
using MasterDataFlow.Network.Packets;

namespace MasterDataFlow.Trading.Ui.Business.Teacher
{
    public class RemoteEnvironment : IDisposable
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

        private CommandWorkflow _commandWorkflow;
        private ServerGate _serverGate;

        public RemoteEnvironment()
        {
            PrepeareSingleRemoteContainer();
        }

        public CommandWorkflow CommandWorkflow
        {
            get { return _commandWorkflow; }
        }

        private void PrepeareSingleRemoteContainer()
        {
            var serverGateKey = new ServiceKey();
            var remoteClientContext = new RemoteClientContextMock(serverGateKey);

            var remoteCommandRunner = new CommandRunner();
            _serverGate = new ServerGate(serverGateKey, remoteClientContext);
            _serverGate.ConnectHub(remoteCommandRunner);

            var remoteContainer = new SimpleContainer();
            remoteCommandRunner.ConnectHub(remoteContainer);

            var runner = new CommandRunner();

            remoteClientContext.SetContract(_serverGate);
            var clientGate = new ClientGate(remoteClientContext);
            runner.ConnectHub(clientGate);

            _commandWorkflow = new CommandWorkflow();
            runner.ConnectHub(_commandWorkflow);

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
                _serverGate.Dispose();
            }
        }
    }
}
