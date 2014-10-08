using System;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class SimpleContainerTests
    {
        private class CommandSuccessDataObject : ICommandDataObject 
        {
            
        }

        private class CommandSuccess : Command<CommandSuccessDataObject>
        {
            public override INextCommandResult<ICommandDataObject> Execute()
            {
                return NextStopCommand();
            }
        }

        private class CommandFaultDataObject : ICommandDataObject
        {

        }

        private class CommandFault : Command<CommandFaultDataObject>
        {
            public override INextCommandResult<ICommandDataObject> Execute()
            {
                throw new Exception();
            }
        }

        private class CommandSubscribeDataObject : ICommandDataObject
        {

        }

        private static int _commandSubscribeOnSubscribedCall = 0;
        private static int _commandSubscribeOnUnsubscribedCall = 0;

        private class CommandSubscribe : Command<CommandSubscribeDataObject>
        {
            public override INextCommandResult<ICommandDataObject> Execute()
            {
                _event.WaitOne();
                return NextStopCommand();
            }

            protected override void OnSubscribed(BaseKey key)
            {
                ++_commandSubscribeOnSubscribedCall;
                _event.Set();
            }

            protected override void OnUnsubscribed(BaseKey key)
            {
                ++_commandSubscribeOnUnsubscribedCall;
                _event.Set();
            }
        }

        private CommandRunner _runner;
        private static ManualResetEvent _event;
        private IContainer _container;

        [TestInitialize]
        public void TestInitialize()
        {
            _container = new SimpleContainer();
            _runner = new CommandRunner();
            _event = new ManualResetEvent(false);
            _commandSubscribeOnSubscribedCall = 0;
            _commandSubscribeOnUnsubscribedCall = 0;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _container.Dispose();
            _runner.Dispose();
            _event.Dispose();
        }

        [TestMethod]
        public void BasicUsageSuccessTest()
        {
            // ARRANGE

            var loopId = Guid.NewGuid();
            var commandDefinition = CommandBuilder.Build<CommandSuccess>().Complete();

            var commandInfo = new CommandInfo
            {
                CommandDefinition = commandDefinition,
                CommandDataObject = new CommandSuccessDataObject(),
                CommandWorkflow = new CommandWorkflow(_runner)
            };
            var callbackStatus = EventLoopCommandStatus.NotStarted;

            // ACT
            _container.Execute(loopId, commandInfo, (id, status, message) =>
            {
                callbackStatus = status;
                _event.Set();
            });

            // ASSERT
            _event.WaitOne(1000);
            Assert.AreEqual(EventLoopCommandStatus.Completed, callbackStatus);
        }

        [TestMethod]
        public void BasicUsageFaultTest()
        {
            // ARRANGE

            var loopId = Guid.NewGuid();
            var commandDefinition = CommandBuilder.Build<CommandFault>().Complete();

            var commandInfo = new CommandInfo
            {
                CommandDefinition = commandDefinition,
                CommandDataObject = new CommandFaultDataObject(),
                CommandWorkflow = new CommandWorkflow(_runner)
            };
            var callbackStatus = EventLoopCommandStatus.NotStarted;

            // ACT
            _container.Execute(loopId, commandInfo, (id, status, message) =>
            {
                callbackStatus = status;
                _event.Set();
            });

            // ASSERT
            _event.WaitOne(1000);
            Assert.AreEqual(EventLoopCommandStatus.Fault, callbackStatus);
        }

        [TestMethod]
        public void SubscribeTest()
        {
            // ARRANGE

            var loopId = Guid.NewGuid();
            var commandDefinition = CommandBuilder.Build<CommandSubscribe>().Complete();

            var workflowKey = new WorkflowKey();

            var commandInfo = new CommandInfo
            {
                CommandDefinition = commandDefinition,
                CommandDataObject = new CommandSubscribeDataObject(),
                CommandWorkflow = new CommandWorkflow(workflowKey, _runner)
            };
            _container.Execute(loopId, commandInfo, (id, status, message) => { });

            // ACT
            _container.Subscribe(workflowKey, new StringKey("test"));

            // ASSERT
            _event.WaitOne(1000);
            Assert.AreEqual(1, _commandSubscribeOnSubscribedCall);
        }

    }
}
