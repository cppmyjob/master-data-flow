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

            public override BaseMessage Execute()
            {
                _event.WaitOne(10000);
                //return NextStopCommand();
                return null;
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



    }
}
