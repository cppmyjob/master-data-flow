﻿using System;
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
    public class CommandWorkflowTests
    {
        private CommandRunner _runner;
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
            _runner = new CommandRunner();
            _event = new ManualResetEvent(false);
            _subscribeCommandOnSubscribedCall = 0;
            _subscribeCommandOnUnsubscribedCall = 0;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _event.Close();
        }

        [TestMethod]
        public void SimpleCommandExecuteTest()
        {
            // ARRANGE
            var container = new SimpleContainer();
            _runner.ConnectHub(container);

            var сommandWorkflow = new CommandWorkflow();
            _runner.ConnectHub(сommandWorkflow);

            // ACT
            сommandWorkflow.Start<ExecuteCommand>(null);


            // ASSERT
            _event.WaitOne(200);
            Assert.AreEqual(1, _executeCommandCall);
        }

        [TestMethod]
        public void DifferentKeysTest()
        {
            // ARRANGE, ACT
            var workflow1 = new CommandWorkflow();
            var workflow2 = new CommandWorkflow();

            // ASSERT
            Assert.AreNotEqual(workflow1.Key, workflow2.Key);
        }

        [TestMethod]
        public void StartPassingCommandTest()
        {
            // ARRANGE
            var container = new SimpleContainer();
            _runner.ConnectHub(container);

            var сommandWorkflow = new CommandWorkflow();
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
            _event.WaitOne(200);
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


        //[TestMethod]
        //public void SubscribePassingTest()
        //{
        //    // ARRANGE
        //    var container = new SimpleContainer();
        //    _runner.AddContainter(container);

        //    var commandDefinition = CommandBuilder.Build<SubscribeCommand>().Complete();
        //    var сommandWorkflow = new CommandWorkflow(_runner);
        //    сommandWorkflow.Register(commandDefinition);

        //    // ACT
        //    сommandWorkflow.Start<SubscribeCommand>(null);

        //    сommandWorkflow.Subscribe(new StringKey("test"));

        //    // ASSERT
        //    _event.WaitOne(1000);
        //    Assert.AreEqual(1, _subscribeCommandOnSubscribedCall);

        //}


        //// TODO to implement
        //[TestMethod]
        //[Ignore]
        //public void StartWithNullDataTest()
        //{
        //    // ARRANGE
        //    var container = new SimpleContainer();
        //    _runner.AddContainter(container);

        //    var commandDefinition = CommandBuilder.Build<SubscribeCommand>().Complete();
        //    var сommandWorkflow = new CommandWorkflow(_runner);
        //    сommandWorkflow.Register(commandDefinition);

        //    // ACT
        //    сommandWorkflow.Start<SubscribeCommand>(null);

        //    сommandWorkflow.Subscribe(new StringKey("test"));

        //    // ASSERT
        //    _event.WaitOne(1000);
        //    Assert.AreEqual(1, _subscribeCommandOnSubscribedCall);

        //}
    }
}
