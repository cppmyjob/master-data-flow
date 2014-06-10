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
    public class CommandDomainTests
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
            var domain1 = new CommandDomain(_runner);
            var domain2 = new CommandDomain(_runner);

            // ASSERT
            Assert.AreNotEqual(Guid.Empty, domain1.Id);
            Assert.AreNotEqual(Guid.Empty, domain2.Id);
            Assert.AreNotEqual(domain1.Id.ToString(), domain2.Id.ToString());
        }


        [TestMethod]
        public void StartCommandTest()
        {
            // ARRANGE
            var container = new SimpleContainer();
            _runner.AddContainter(container);


            var commandDefinition = CommandBuilder.Build<PassingCommand>().Complete();
            var сommandDomain = new CommandDomain(_runner);
            сommandDomain.Register(commandDefinition);

            // ACT
            var newId = Guid.NewGuid();
            Guid callbackId = Guid.Empty;
            var callbackStatus = EventLoopCommandStatus.NotStarted;
            ILoopCommandMessage callbackMessage = null;
            var originalId = сommandDomain.Start<PassingCommand>(new PassingCommandDataObject(newId), (id, status, message) =>
            {
                callbackId = id;
                callbackStatus = status;
                callbackMessage = message;
                _event.Set();
            });

            // ASSERT
            _event.WaitOne(200);
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
