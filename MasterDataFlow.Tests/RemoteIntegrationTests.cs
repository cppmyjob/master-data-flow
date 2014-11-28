using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Actions.UploadType;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Network;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class RemoteIntegrationTests
    {
        private EventWaitHandle _eventWaitHandle;

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

        public class ExecuteCommand : Command<CommandDataObjectStub>
        {

            public override BaseMessage Execute()
            {
                using (var eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset,
                        "AE2E34E2-A03D-4B53-930D-A15CA44BCF21"))
                {
                    eventWaitHandle.Set();
                }
                return Stop(null);
            }

            protected override void OnSubscribed(BaseKey key)
            {
            }

            protected override void OnUnsubscribed(BaseKey key)
            {
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
        }

        [TestMethod]
        public void RemoteCommandIsExecutedTest()
        {
            // ARRANGE
            var serverGateKey = new ServiceKey();
            var remoteClientContext = new RemoteClientContextMock(serverGateKey);

            var remoteCommandRunner = new CommandRunnerHub();
            var serverGate = new ServerGate(serverGateKey, remoteClientContext);
            serverGate.ConnectHub(remoteCommandRunner);
            
            var remoteContainer = new SimpleContainerHub();
            remoteCommandRunner.ConnectHub(remoteContainer);

            var runner = new CommandRunnerHub();

            remoteClientContext.SetContract(serverGate);
            var clientGate = new ClientGate(remoteClientContext);
            runner.ConnectHub(clientGate);

            var сommandWorkflow = new CommandWorkflowHub();
            runner.ConnectHub(сommandWorkflow);

            SendUploadResponse(serverGate, typeof(ExecuteCommand), сommandWorkflow.Key);

            // ACT
            сommandWorkflow.Start<ExecuteCommand>(null);

            // ASSERT
            var result = _eventWaitHandle.WaitOne(200);
            Assert.IsTrue(result);
        }


        [TestMethod]
        public void RemoteCommandStopEventReturnedTest()
        {
            // ARRANGE
            var serverGateKey = new ServiceKey();
            var remoteClientContext = new RemoteClientContextMock(serverGateKey);

            var remoteCommandRunner = new CommandRunnerHub();
            var serverGate = new ServerGate(serverGateKey, remoteClientContext);
            serverGate.ConnectHub(remoteCommandRunner);

            var remoteContainer = new SimpleContainerHub();
            remoteCommandRunner.ConnectHub(remoteContainer);

            var runner = new CommandRunnerHub();

            remoteClientContext.SetContract(serverGate);
            var clientGate = new ClientGate(remoteClientContext);
            runner.ConnectHub(clientGate);

            var сommandWorkflow = new CommandWorkflowHub();
            runner.ConnectHub(сommandWorkflow);

            SendUploadResponse(serverGate, typeof(PassingCommand), сommandWorkflow.Key);

            var callStopCommand = 0;
            BaseMessage callMessage = null;
            сommandWorkflow.MessageRecieved += (key, message) =>
            {
                ++callStopCommand;
                callMessage = message;
                using (var eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset,
                        "AE2E34E2-A03D-4B53-930D-A15CA44BCF21"))
                {
                    eventWaitHandle.Set();
                }
            };

            // ACT
            var newId = Guid.NewGuid();
            var commandKey = сommandWorkflow.Start<PassingCommand>(new PassingCommandDataObject(newId));

            // ASSERT
            _eventWaitHandle.WaitOne(200);

            Assert.AreEqual(1, callStopCommand);
            Assert.IsNotNull(callMessage);
            Assert.IsTrue(callMessage is StopCommandMessage);
            var stopCommand = callMessage as StopCommandMessage;
            Assert.AreEqual(commandKey, stopCommand.Key);

            Assert.IsNotNull(stopCommand.Data);
            var passingCommand = stopCommand.Data as PassingCommandDataObject;
            Assert.IsNotNull(passingCommand);
            Assert.AreEqual(newId, passingCommand.Id);
        }

        private void SendUploadResponse(IHub hub, Type type, BaseKey workflowKey)
        {
            string path = type.Assembly.Location;
            var assemblyFilename = Path.GetFileName(path);
            using (var stream = File.OpenRead(path))
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                var responseAction = new UploadTypeResponseAction
                {
                    TypeName = type.AssemblyQualifiedName,
                    AssemblyData = buffer,
                    AssemblyName = assemblyFilename,
                    WorkflowKey = workflowKey.Key
                };
                hub.Send(new Packet(new ServiceKey(), hub.Key, responseAction));
            }
        }
    }
}
