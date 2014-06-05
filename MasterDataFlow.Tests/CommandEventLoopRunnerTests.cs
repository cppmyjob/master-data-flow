using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
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
                        waitCallBack(id, EventLoopCommandStatus.Completed, null);
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
            Assert.IsTrue(callbackMessage is INextCommandResult<CommandDataObjectStub>);
        }
    }
}
