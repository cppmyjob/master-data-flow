using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class CommandWorkflowTests
    {
        private CommandRunner _runner;
        private static ManualResetEvent _event;

        private static int _subscribeCommandOnSubscribedCall = 0;
        private static int _subscribeCommandOnUnsubscribedCall = 0;

        public class SubscribeCommand : Command<CommandDataObjectStub>
        {

            public override INextCommandResult<ICommandDataObject> Execute()
            {
                _event.WaitOne(10000);
                return NextStopCommand();
            }

            protected override void OnSubscribed(BaseKey key)
            {
                ++_subscribeCommandOnSubscribedCall;
                _event.Set();
            }

            protected override void OnUnsubscribed(BaseKey key)
            {
                ++_subscribeCommandOnUnsubscribedCall;
                _event.Set();
            }
        }


        [TestInitialize]
        public void TestInitialize()
        {
            _runner = new CommandRunner();
            _event = new ManualResetEvent(false);
            _subscribeCommandOnSubscribedCall = 0;
            _subscribeCommandOnUnsubscribedCall = 0;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _runner.Dispose();
            _event.Dispose();
        }


        [TestMethod]
        public void SubscribePassingTest()
        {
            // ARRANGE
            var container = new SimpleContainer();
            _runner.AddContainter(container);

            var commandDefinition = CommandBuilder.Build<SubscribeCommand>().Complete();
            var сommandWorkflow = new CommandWorkflow(_runner);
            сommandWorkflow.Register(commandDefinition);

            // ACT
            сommandWorkflow.Start<SubscribeCommand>(null);

            сommandWorkflow.Subscribe(new StringKey("test"));

            // ASSERT
            _event.WaitOne(1000);
            Assert.AreEqual(1, _subscribeCommandOnSubscribedCall);

        }


        // TODO to implement
        [TestMethod]
        [Ignore]
        public void StartWithNullDataTest()
        {
            // ARRANGE
            var container = new SimpleContainer();
            _runner.AddContainter(container);

            var commandDefinition = CommandBuilder.Build<SubscribeCommand>().Complete();
            var сommandWorkflow = new CommandWorkflow(_runner);
            сommandWorkflow.Register(commandDefinition);

            // ACT
            сommandWorkflow.Start<SubscribeCommand>(null);

            сommandWorkflow.Subscribe(new StringKey("test"));

            // ASSERT
            _event.WaitOne(1000);
            Assert.AreEqual(1, _subscribeCommandOnSubscribedCall);

        }

        [TestMethod]
        public void IdTest()
        {
            // ARRANGE, ACT
            var workflow1 = new CommandWorkflow(_runner);
            var workflow2 = new CommandWorkflow(_runner);

            // ASSERT
            Assert.AreNotEqual(Guid.Empty, workflow1.Key);
            Assert.AreNotEqual(Guid.Empty, workflow2.Key);
            Assert.AreNotEqual(workflow1.Key.ToString(), workflow2.Key.ToString());
        }


        [TestMethod]
        public void StartCommandTest()
        {
            // ARRANGE
            var container = new SimpleContainer();
            _runner.AddContainter(container);


            var commandDefinition = CommandBuilder.Build<PassingCommand>().Complete();
            var сommandWorkflow = new CommandWorkflow(_runner);
            сommandWorkflow.Register(commandDefinition);

            // ACT
            var newId = Guid.NewGuid();
            Guid callbackId = Guid.Empty;
            var callbackStatus = EventLoopCommandStatus.NotStarted;
            ILoopCommandMessage callbackMessage = null;
            сommandWorkflow.MessageRecieved += (id, status, message) =>
            {
                callbackId = id;
                callbackStatus = status;
                callbackMessage = message;
                _event.Set();
            };
            var originalId = сommandWorkflow.Start<PassingCommand>(new PassingCommandDataObject(newId));

            // ASSERT
            _event.WaitOne(1000);
            Assert.AreEqual(originalId, callbackId);
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
