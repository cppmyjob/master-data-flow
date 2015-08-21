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
        private static GeneticCellDataObject _dataObject;

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

        [Serializable]
        public class MockGeneticItem : GeneticItem
        {
            public MockGeneticItem(GeneticInitData initData) : base(initData)
            {
            }

            protected override double CreateValue(double random)
            {
                return random;
            }
        }

        [Serializable]
        public class GeneticCellDataObjectMock : GeneticCellDataObject
        {
        }

        public class GeneticCellCommandMock : GeneticCellCommand
        {
            public override BaseMessage Execute()
            {
                Console.WriteLine("GeneticCellCommandMock::BaseExecute");
                var result = base.Execute();
                //_dataObject = DataObject;
                //using (var eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset,
                //        "AE2E34E2-A03D-4B53-930D-A15CA44BCF21"))
                //{
                //    eventWaitHandle.Set();
                //}
                return result;
            }

            protected override GeneticItem CreateItem(GeneticInitData initData)
            {
                return new MockGeneticItem(initData);
            }

            public override double CalculateFitness(GeneticItem item, int processor)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void GeneticCellInitDataPassingTest()
        {
            // ARRANGE
            var remote = new RemoteEvironment();
            var сommandWorkflow = remote.PrepeareSingleRemoteContainer();

            // ACT
            var initData = new GeneticCellInitData(100, 30, 50);
            var dataObject = new GeneticCellDataObjectMock
            {
                CellInitData = initData,
            };
            сommandWorkflow.Start<GeneticCellCommandMock>(dataObject);

            // ASSERT
            var result = _eventWaitHandle.WaitOne(2000);
            Assert.IsTrue(result);
        }

    }
}
