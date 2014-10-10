using System;
using System.Threading;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
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


        public class ExecuteCommand : Command<CommandDataObjectStub>
        {

            public override INextCommandResult<ICommandDataObject> Execute()
            {
                ++_executeCommandCall;
                _event.Set();
                return NextStopCommand();
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
            _runner.Dispose();
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
            сommandWorkflow.Start<SubscribeCommand>(null);


            // ASSERT
            _event.WaitOne(1000);
            Assert.AreEqual(1, _executeCommandCall);

        }
    }
}
