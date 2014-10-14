using System;
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
    public class CommandWorkflowHubTests
    {
        private CommandRunnerHub _runner;
        private static ManualResetEvent _event;

        private static int _executeCommandCall = 0;
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
            _runner = new CommandRunnerHub();
            _event = new ManualResetEvent(false);
            _subscribeCommandOnSubscribedCall = 0;
            _subscribeCommandOnUnsubscribedCall = 0;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _event.Dispose();
        }

        [TestMethod]
        public void SimpleCommandExecuteTest()
        {
            // ARRANGE
            var container = new SimpleContainerHub();
            _runner.ConnectHub(container);

            var commandDefinition = CommandBuilder.Build<ExecuteCommand>().Complete();
            var сommandWorkflow = new CommandWorkflowHub();
            сommandWorkflow.Register(commandDefinition);
            _runner.ConnectHub(сommandWorkflow);

            // ACT
            сommandWorkflow.Start<ExecuteCommand>(null);


            // ASSERT
            _event.WaitOne(10000);
            Assert.AreEqual(1, _executeCommandCall);

        }

        [TestMethod]
        public void DifferentKeysTest()
        {
            // ARRANGE, ACT
            var workflow1 = new CommandWorkflowHub();
            var workflow2 = new CommandWorkflowHub();

            // ASSERT
            Assert.AreNotEqual(workflow1.Key, workflow2.Key);
        }

        [TestMethod]
        public void StartPassingCommandTest()
        {
            // ARRANGE
            var container = new SimpleContainerHub();
            _runner.ConnectHub(container);

            var commandDefinition = CommandBuilder.Build<PassingCommand>().Complete();
            var сommandWorkflow = new CommandWorkflowHub();
            сommandWorkflow.Register(commandDefinition);
            _runner.ConnectHub(сommandWorkflow);

            // ACT
            var newId = Guid.NewGuid();
            BaseKey callbackSenderKey = null;
            BaseMessage callbackMessage = null;
            сommandWorkflow.MessageRecieved += (senderKey, message) =>
            {
                callbackSenderKey = senderKey;
                callbackMessage = message;
                _event.Set();
            };
            var commandKey = сommandWorkflow.Start<PassingCommand>(new PassingCommandDataObject(newId));

            // ASSERT
            _event.WaitOne(1000);
            Assert.AreEqual(container.Key, callbackSenderKey);
            var commandMessage = callbackMessage as CommandMessage;
            Assert.IsNotNull(commandMessage);
            Assert.AreEqual(commandKey, commandMessage.Key);

            Assert.IsTrue(callbackMessage is StopCommandMessage);
            var dataMessage = callbackMessage as StopCommandMessage;
            Assert.IsNotNull(dataMessage.Data);
            Assert.IsTrue(dataMessage.Data is PassingCommandDataObject);
            Assert.AreEqual(newId, ((PassingCommandDataObject)dataMessage.Data).Id);
        }
    }
}
