using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;
using MasterDataFlow.Results;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MasterDataFlow.Tests
{

    [TestClass]
    public class CommandRunnerTests
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
                        waitCallBack(id, EventLoopCommandStatus.Completed, new ResultCommandMessage(new StopCommandResult()));
                        _event.Set();

                    });
            _runner.AddContainter(container.Object);
            var commandDefinition = new CommandDefinition(typeof (CommandStub));

            // ACT
            _runner.Run(new CommandDomain(_runner),  commandDefinition);

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
            var originalId = _runner.Run(new CommandDomain(_runner), commandDefinition);

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
            var originalId = _runner.Run(new CommandDomain(_runner), commandDefinition, null, (id, status, message) =>
            {
                callbackId = id;
                _event.Set();
            });

            // ASSERT
            _event.WaitOne(1000);
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
            var originalId = _runner.Run(new CommandDomain(_runner), commandDefinition, null, (id, status, message) =>
            {
                callbackMessage = message;
                _event.Set();
            });

            // ASSERT
            _event.WaitOne(1000);
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
            var originalId = _runner.Run(new CommandDomain(_runner), commandDefinition, new PassingCommandDataObject(newId), (id, status, message) =>
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


        [TestMethod]
        public void TwoCommandsTest()
        {
            // ARRANGE
            var container = new SimpleContainer();
            _runner.AddContainter(container);

            var definition1 = new CommandDefinition(typeof(Command1));
            var definition2 = new CommandDefinition(typeof(Command2));
            var сommandDomain = new CommandDomain(_runner);
            сommandDomain.Register(definition1);
            сommandDomain.Register(definition2);

            // ACT
            var callbackId = new Guid[2];
            var callbackStatus = new EventLoopCommandStatus[2];
            var callbackMessage = new ILoopCommandMessage[2];
            var callCount = 0;
            var originalId = _runner.Run(сommandDomain, definition1, new Command1DataObject(), (id, status, message) =>
            {

                callbackId[callCount] = id;
                callbackStatus[callCount] = status;
                callbackMessage[callCount] = message;

                ++callCount;
                if (callCount == 2)
                    _event.Set();
            });


            // ASSERT
            _event.WaitOne(1000);
            Assert.AreEqual(2, callCount);

            Assert.AreEqual(EventLoopCommandStatus.Completed, callbackStatus[0]);
            Assert.AreEqual(EventLoopCommandStatus.Completed, callbackStatus[1]);

            Assert.IsTrue(callbackMessage[0] is NextCommandMessage);
            Assert.IsTrue(callbackMessage[1] is DataCommandMessage);
            Assert.IsNull((callbackMessage[1] as DataCommandMessage).Data);

            Assert.AreNotEqual(Guid.Empty, callbackId[0]);
            Assert.AreNotEqual(Guid.Empty, callbackId[1]);

            var secondId = (callbackMessage[0] as NextCommandMessage).LoopId;
            Assert.AreEqual(originalId, callbackId[0]);
            Assert.AreEqual(secondId, callbackId[1]);

        }
    }
}
