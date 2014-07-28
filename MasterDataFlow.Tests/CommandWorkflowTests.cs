using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class CommandWorkflowTests
    {
        private CommandRunner _runner;
        private ManualResetEvent _event;

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
        public void IdTests()
        {
            // ARRANGE, ACT
            var workflow1 = new CommandWorkflow(_runner);
            var workflow2 = new CommandWorkflow(_runner);

            // ASSERT
            Assert.AreNotEqual(Guid.Empty, workflow1.Id);
            Assert.AreNotEqual(Guid.Empty, workflow2.Id);
            Assert.AreNotEqual(workflow1.Id.ToString(), workflow2.Id.ToString());
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
            var originalId = сommandWorkflow.Start<PassingCommand>(new PassingCommandDataObject(newId), (id, status, message) =>
            {
                callbackId = id;
                callbackStatus = status;
                callbackMessage = message;
                _event.Set();
            });

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
