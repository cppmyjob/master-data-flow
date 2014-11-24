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
    public class RemoteFixtures
    {
        private EventWaitHandle _eventWaitHandle;

        public class RemoteClientContextMock : IClientContext
        {
            private ServerGate _serverGate;

            public RemoteClientContextMock(ServerGate serverGate)
            {
                _serverGate = serverGate;
            }


            public IGateContract Contract
            {
                get { return _serverGate; } 
            }

            public BaseKey ServerGateKey
            {
                get { return _serverGate.Key; }
            }

            public bool IsNeedSendKey
            {
                get { return false; }
            }

            public void Dispose()
            {
                
            }


            public event GateCallbackPacketRecievedHandler GateCallbackPacketRecieved;

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
            _eventWaitHandle.Dispose();
        }

        [TestMethod]
        public void RemoteCommandBasicUsage()
        {
            // ARRANGE
            var remoteCommandRunner = new CommandRunnerHub();
            var serverGate = new ServerGate();
            serverGate.ConnectHub(remoteCommandRunner);
            var remoteClientContext = new RemoteClientContextMock(serverGate);
            var remoteContainer = new SimpleContainerHub();
            remoteCommandRunner.ConnectHub(remoteContainer);

            var runner = new CommandRunnerHub();

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
