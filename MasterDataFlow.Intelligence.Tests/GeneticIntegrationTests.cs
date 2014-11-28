using System;
using System.IO;
using System.Threading;
using MasterDataFlow.Actions.UploadType;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Network;
using MasterDataFlow.Network.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Intelligence.Tests
{
    [TestClass]
    public class GeneticIntegrationTests
    {
        private EventWaitHandle _eventWaitHandle;
        private GeneticCellDataObject _dataObject;

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


        [TestInitialize]
        public void TestInitialize()
        {
            _eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "AE2E34E2-A03D-4B53-930D-A15CA44BCF21");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _eventWaitHandle.Close();
            _dataObject = null;
        }

        public class GeneticCellDataObjectMock : GeneticCellDataObject
        {
        }


        public class GeneticCellCommandMock : GeneticCellCommand
        {
            protected override BaseMessage BaseExecute()
            {
                //_dataObject = DataObject;
                using (var eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset,
                        "AE2E34E2-A03D-4B53-930D-A15CA44BCF21"))
                {
                    eventWaitHandle.Set();
                }
                return base.BaseExecute();
            }
        }

        [TestMethod]
        public void GeneticCellInitDataPassingTest()
        {
            // ARRANGE
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

            // ACT
            var initData = new GeneticCellInitData(100, 30, 50);
            var dataObject = new GeneticCellDataObjectMock
            {
                CellInitData = initData
            };
            сommandWorkflow.Start<GeneticCellCommandMock>(dataObject);

            // ASSERT
            var result = _eventWaitHandle.WaitOne(20000);
            Assert.IsTrue(result);
        }

    }
}
