using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Actions;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MasterDataFlow.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), TestClass]
    public class ClientGateTests
    {
        private ManualResetEvent _event;

        private const string ServerGateId = "1db907fb-77c7-465f-bd60-031107374727";
        private const string RunnerId = "4BB7C70B-F088-4D09-B06D-CC1CF64580CE";
        private const string WorkflowId = "C2B980FF-7C4D-4B43-9935-497218492783";

        private class RemoteClientContextMock : IClientContext
        {
            private readonly BaseKey _serverGateKey;
            private readonly IGateContract _contract;

            public RemoteClientContextMock(BaseKey serverGateKey, IGateContract contract)
            {
                _serverGateKey = serverGateKey;
                _contract = contract;
            }

            public IGateContract Contract
            {
                get { return _contract; }
            }

            public BaseKey ServerGateKey
            {
                get { return _serverGateKey; }
            }

            public void Dispose()
            {
                
            }
        }

        private class RemoteHostContractMock
        {

            private readonly Mock<IGateContract> _contract;
            private RemotePacket _packet;
            private int _calls = 0;

            public RemoteHostContractMock(ManualResetEvent @event)
            {
                _contract = new Mock<IGateContract>();
                _contract.Setup(t => t.Send(It.IsAny<RemotePacket>())).Callback<RemotePacket>((packet) =>
                {
                    _packet = packet;
                    _calls = _calls + 1;
                    @event.Set();
                });
            }

            public IGateContract Object
            {
                get { return _contract.Object; }
            }

            public RemotePacket Packet
            {
                get { return _packet; }
            }

            public int Calls
            {
                get { return _calls; }
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _event = new ManualResetEvent(false);

        }

        [TestCleanup]
        public void TestCleanup()
        {
            _event.Dispose();
        }

        [TestMethod]
        public void ExecuteValidParametersTest()
        {
            // ARRANGE
            var serverGateKey = new ServiceKey(new Guid(ServerGateId));

            var contract = new RemoteHostContractMock(_event);
            var context = new RemoteClientContextMock(serverGateKey, contract.Object);
            var clientGate = new ClientGate(context);

            var workflowKey = new WorkflowKey(new Guid(WorkflowId));
            const string commandId = "8CC9A7EC-AF69-4EBC-BF2C-072E85212BB1";
            var commandKey = new CommandKey(new Guid(commandId));

            const string dataId = "17135B70-68E8-4479-BE68-1F220B04011A";

            var info = new RemoteExecuteCommandAction.Info
            {
                CommandType = typeof(PassingCommand).AssemblyQualifiedName,
                DataObject =  Serialization.Serializator.Serialize(new PassingCommandDataObject(new Guid(dataId))),
                DataObjectType = typeof(PassingCommandDataObject).AssemblyQualifiedName,
                WorkflowKey = workflowKey.Key,
                CommandKey = commandKey.Key
            };

            var runnerKey = new ServiceKey(new Guid(RunnerId));
            // ACT
            var action = new RemoteExecuteCommandAction { CommandInfo = info };
            var packet = new Packet(runnerKey, serverGateKey, action);
            clientGate.Send(packet);

            // ASSERT
            _event.WaitOne(1000);
            Assert.AreEqual(1, contract.Calls);
            Assert.AreEqual(runnerKey.Key, contract.Packet.SenderKey);
            Assert.AreEqual(serverGateKey.Key, contract.Packet.RecieverKey);
            Assert.AreEqual("MasterDataFlow.Actions.RemoteExecuteCommandAction, MasterDataFlow, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", contract.Packet.TypeName);

            var bodyType = Type.GetType(contract.Packet.TypeName);
            var remoteAction = (RemoteExecuteCommandAction)Serialization.Serializator.Deserialize(bodyType, (string)contract.Packet.Body);

            Assert.AreEqual(info.CommandType, remoteAction.CommandInfo.CommandType);
            Assert.AreEqual(info.DataObject, remoteAction.CommandInfo.DataObject);
            Assert.AreEqual(info.DataObjectType, remoteAction.CommandInfo.DataObjectType);
            Assert.AreEqual(info.WorkflowKey, remoteAction.CommandInfo.WorkflowKey);
            Assert.AreEqual(info.CommandKey, remoteAction.CommandInfo.CommandKey);
        }
    }
}
