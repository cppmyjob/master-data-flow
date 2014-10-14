using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Remote;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class RemoteFixtures
    {
        private CommandRunner _runner;
        private ManualResetEvent _event;

        private const string LoopId = "1db907fb-77c7-465f-bd60-031107374727";
        private const string WorkflowId = "C2B980FF-7C4D-4B43-9935-497218492783";


        private class RemoteClientContextMock : RemoteClientContext
        {
            private RemoteHost _remoteHost;
            private RemoteHostController _remoteController;

            public RemoteClientContextMock(BaseKey serverGateKey)
                : base(serverGateKey)
            {
                
            }


            protected override IRemoteHostContract CreateContract()
            {
                if (_remoteController == null)
                {
                    _remoteHost = new RemoteHost();
                    _remoteHost.AddContainter(new SimpleContainer());
                    _remoteController = new RemoteHostController(_remoteHost, this);
                }
                return _remoteController;
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _runner = new CommandRunner();
            _event = new ManualResetEvent(false);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _runner.Dispose();
            _event.Dispose();
        }

        [TestMethod]
        public void RemoteCommandBasicUsage()
        {
            // ARRANGE
            IRemoteClientContext context = new RemoteClientContextMock(new ServiceKey());
            var container = new RemoteContainer(context);
            _runner.AddContainter(container);
            var commandDefinition = new CommandDefinition(typeof(PassingCommand));
            const string commandId = "8CC9A7EC-AF69-4EBC-BF2C-072E85212BB1";
            var commandKey = new CommandKey(new Guid(commandId));

            // ACT
            int calls = 0;
            var newId = Guid.NewGuid();
            Guid callbackId = Guid.Empty;
            var callbackStatus = EventLoopCommandStatus.NotStarted;
            ILoopCommandMessage callbackMessage = null;
            var workflow = new CommandWorkflow(_runner);
            workflow.MessageRecieved += (id, status, message) =>
            {
                callbackId = id;
                callbackStatus = status;
                callbackMessage = message;
                ++calls;
                if (calls == 2)
                    _event.Set();
            };

            _runner.Run(workflow, commandKey, commandDefinition, new PassingCommandDataObject(newId));

            // ASSERT
            _event.WaitOne(1000);
            Assert.Fail();
            //Assert.AreEqual(originalId, callbackId);
            Assert.AreEqual(EventLoopCommandStatus.Completed, callbackStatus);
            Assert.IsNotNull(callbackMessage);
            Assert.IsTrue(callbackMessage is DataCommandMessage);
            var dataMessage = callbackMessage as DataCommandMessage;
            Assert.IsNotNull(dataMessage.Data);
            Assert.IsTrue(dataMessage.Data is PassingCommandDataObject);
            Assert.AreEqual(newId, ((PassingCommandDataObject)dataMessage.Data).Id);
            
        }
    }
}
