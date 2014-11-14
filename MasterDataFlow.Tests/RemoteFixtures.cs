using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
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
        private static ManualResetEvent _event;

        private static int _executeCommandCall = 0;

        private const string LoopId = "1db907fb-77c7-465f-bd60-031107374727";
        private const string WorkflowId = "C2B980FF-7C4D-4B43-9935-497218492783";


        private class RemoteClientContextMock : IClientContext
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

            public void Dispose()
            {
                
            }


            public event GateCallbackPacketRecievedHandler GateCallbackPacketRecieved;
        }

        public class ExecuteCommand : Command<CommandDataObjectStub>
        {

            public override BaseMessage Execute()
            {
                ++_executeCommandCall;
                _event.Set();
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
            _event = new ManualResetEvent(false);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _event.Dispose();
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

            var commandDefinition = CommandBuilder.Build<ExecuteCommand>().Complete();
            var сommandWorkflow = new CommandWorkflowHub();
            сommandWorkflow.Register(commandDefinition);
            runner.ConnectHub(сommandWorkflow);

            // ACT
            сommandWorkflow.Start<ExecuteCommand>(null);

            // ASSERT
            _event.WaitOne(200);
            Assert.AreEqual(1, _executeCommandCall);

        }
    }
}
