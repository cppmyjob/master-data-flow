using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;
using MasterDataFlow.Results;
using MasterDataFlow.Tests.Mocks;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MasterDataFlow.Tests
{

    [TestClass]
    public class CommandEventLoopRunnerTests
    {
        private CommandEventLoopRunner _runner;
        private CommandDomain _сommandDomain;
        private ManualResetEvent _event;

        [TestInitialize]
        public void TestInitialize()
        {
            _сommandDomain = new CommandDomain();
            _runner = new CommandEventLoopRunner(_сommandDomain);
            _event = new ManualResetEvent(false);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _runner.Dispose();
        }


        [TestMethod]
        public void ContainerExecutedTest()
        {
            // ARRANGE
            int callCount = 0;
            var container = new Mock<BaseContainter>();
            container.Setup(
                t => t.Execute(It.IsAny<Guid>(), It.IsAny<ILoopCommandData>(), It.IsAny<EventLoopCallback>()))
                .Callback<Guid, ILoopCommandData, EventLoopCallback>(
                    (id, data, waitCallBack) =>
                    {
                        ++callCount;
                        _event.Set();
                    });
            _runner.AddContainter(container.Object);
            var commandDefinition = new CommandDefinition(typeof (CommandStub));

            // ACT
            _runner.Run(commandDefinition);

            // ASSERT
            _event.WaitOne(1000);
            Assert.AreEqual(1, callCount);
        }

        [TestMethod]
        public void ContainerPassingValidIdTest()
        {
            // ARRANGE
            Guid containerId = Guid.Empty;
            var container = new Mock<BaseContainter>();
            container.Setup(
                t => t.Execute(It.IsAny<Guid>(), It.IsAny<ILoopCommandData>(), It.IsAny<EventLoopCallback>()))
                .Callback<Guid, ILoopCommandData, EventLoopCallback>(
                    (id, data, waitCallBack) =>
                    {
                        containerId = id;
                        _event.Set();
                    });
            _runner.AddContainter(container.Object);
            var commandDefinition = new CommandDefinition(typeof(CommandStub));

            // ACT
            var originalId = _runner.Run(commandDefinition);

            // ASSERT
            _event.WaitOne(1000);
            Assert.AreNotEqual(Guid.Empty, containerId);
            Assert.AreEqual(originalId, containerId);
        }

        [TestMethod]
        public void ContainerReturnCallbackTest()
        {
            // ARRANGE
            var container = new Mock<BaseContainter>();
            container.Setup(
                t => t.Execute(It.IsAny<Guid>(), It.IsAny<ILoopCommandData>(), It.IsAny<EventLoopCallback>()))
                .Callback<Guid, ILoopCommandData, EventLoopCallback>(
                    (id, data, waitCallBack) =>
                    {
                        waitCallBack(id, EventLoopCommandStatus.Completed, new ResultCommandMessage(new StopCommandResult()));
                    });
            _runner.AddContainter(container.Object);
            var commandDefinition = new CommandDefinition(typeof(CommandStub));

            // ACT
            Guid callbackId = Guid.Empty;
            var originalId = _runner.Run(commandDefinition, null, (id, status, message) =>
            {
                callbackId = id;
                _event.Set();
            });

            // ASSERT
            _event.WaitOne(100);
            Assert.AreNotEqual(Guid.Empty, callbackId);
            Assert.AreEqual(originalId, callbackId);
        }

        [TestMethod]
        public void ContainerCommandExecuteTest()
        {
            // ARRANGE
            var container = new SimpleContainer();
            _runner.AddContainter(container);
            var commandDefinition = new CommandDefinition(typeof(CommandStub));

            // ACT
            object callbackMessage = null;
            var originalId = _runner.Run(commandDefinition, null, (id, status, message) =>
            {
                callbackMessage = message;
                _event.Set();
            });

            // ASSERT
            _event.WaitOne(100);
            Assert.IsNotNull(callbackMessage);
            Assert.IsTrue(callbackMessage is DataCommandMessage);
            Assert.IsNull((callbackMessage as DataCommandMessage).Data);
        }


        [TestMethod]
        public void PassingInputDataToResultTest()
        {
            // ARRANGE
            var container = new SimpleContainer();
            _runner.AddContainter(container);
            var commandDefinition = new CommandDefinition(typeof(PassingCommand));

            // ACT
            var newId = Guid.NewGuid();
            Guid callbackId = Guid.Empty;
            var callbackStatus = EventLoopCommandStatus.NotStarted;
            ILoopCommandMessage callbackMessage = null;
            var originalId = _runner.Run(commandDefinition, new PassingCommandDataObject(newId), (id, status, message) =>
            {
                callbackId = id;
                callbackStatus = status;
                callbackMessage = message;
                _event.Set();
            });

            // ASSERT
            _event.WaitOne(100);
            Assert.AreEqual(originalId, callbackId);
            Assert.AreEqual(EventLoopCommandStatus.Completed, callbackStatus);
            Assert.IsNotNull(callbackMessage);
            Assert.IsTrue(callbackMessage is DataCommandMessage);
            var dataMessage = callbackMessage as DataCommandMessage;
            Assert.IsNotNull(dataMessage.Data);
            Assert.IsTrue(dataMessage.Data is PassingCommandDataObject);
            Assert.AreEqual(newId, ((PassingCommandDataObject)dataMessage.Data).Id);
        }


        [TestMethod]
        public void TwoCommandsTest()
        {
            // ARRANGE
            var container = new SimpleContainer();
            _runner.AddContainter(container);

            var definition1 = new CommandDefinition(typeof(Command1));
            var definition2 = new CommandDefinition(typeof(Command2));
            //_сommandDomain.Register(definition1);
            _сommandDomain.Register(definition2);

            // ACT
            Guid callbackId = Guid.Empty;
            var callbackStatus = EventLoopCommandStatus.NotStarted;
            ILoopCommandMessage callbackMessage = null;
            var originalId = _runner.Run(definition1, new Command1DataObject(), (id, status, message) =>
            {
                callbackId = id;
                callbackStatus = status;
                callbackMessage = message;
                _event.Set();
            });


            // ASSERT
            _event.WaitOne(1000000);
            Assert.AreNotEqual(Guid.Empty, callbackId);
            Assert.AreEqual(originalId, callbackId);
            Assert.AreEqual(EventLoopCommandStatus.Completed, callbackStatus);
            Assert.IsNull((callbackMessage as DataCommandMessage).Data);
        }
    }
}
