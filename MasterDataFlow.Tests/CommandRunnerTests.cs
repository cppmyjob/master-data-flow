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
using Moq;

namespace MasterDataFlow.Tests
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), TestClass]
    public class CommandRunnerTests
    {
        //private CommandRunner _runner;
        private ManualResetEvent _event;

        [TestInitialize]
        public void TestInitialize()
        {
            //_runner = new CommandRunner();
            _event = new ManualResetEvent(false);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            //_runner.Dispose();
            _event.Dispose();
        }

        // TODO Restore
        //[TestMethod]
        //public void ContainerExecutedTest()
        //{
        //    // ARRANGE
        //    int callCount = 0;
        //    var container = new Mock<IContainer>();
        //    container.Setup(
        //        t => t.Execute(It.IsAny<Guid>(), It.IsAny<ILoopCommandData>(), It.IsAny<EventLoopCallback>()))
        //        .Callback<Guid, ILoopCommandData, EventLoopCallback>(
        //            (id, data, waitCallBack) =>
        //            {
        //                ++callCount;
        //                waitCallBack(id, EventLoopCommandStatus.Completed, new ResultCommandMessage(new StopCommandResult()));
        //                _event.Set();

        //            });
        //    _runner.AddContainter(container.Object);
        //    var commandDefinition = new CommandDefinition(typeof (CommandStub));
        //    const string commandId = "8CC9A7EC-AF69-4EBC-BF2C-072E85212BB1";
        //    var commandKey = new CommandKey(new Guid(commandId));

        //    // ACT
        //    _runner.Run(new CommandWorkflow(_runner), commandKey, commandDefinition);

        //    // ASSERT
        //    _event.WaitOne(1000);
        //    Assert.AreEqual(1, callCount);
        //}

        // TODO Restore
        //[TestMethod]
        //public void ContainerPassingValidIdTest()
        //{
        //    // ARRANGE
        //    Guid containerId = Guid.Empty;
        //    var container = new Mock<IContainer>();
        //    container.Setup(
        //        t => t.Execute(It.IsAny<Guid>(), It.IsAny<ILoopCommandData>(), It.IsAny<EventLoopCallback>()))
        //        .Callback<Guid, ILoopCommandData, EventLoopCallback>(
        //            (id, data, waitCallBack) =>
        //            {
        //                containerId = id;
        //                _event.Set();
        //            });
        //    _runner.AddContainter(container.Object);
        //    var commandDefinition = new CommandDefinition(typeof(CommandStub));
        //    const string commandId = "8CC9A7EC-AF69-4EBC-BF2C-072E85212BB1";
        //    var commandKey = new CommandKey(new Guid(commandId));

        //    // ACT
        //    var originalId = _runner.Run(new CommandWorkflow(_runner), commandKey, commandDefinition);

        //    // ASSERT
        //    _event.WaitOne(1000);
        //    Assert.AreNotEqual(Guid.Empty, containerId);
        //    Assert.AreEqual(originalId, containerId);
        //}

        // TODO Restore
        //[TestMethod]
        //public void ContainerReturnCallbackTest()
        //{
        //    // ARRANGE
        //    var container = new Mock<IContainer>();
        //    container.Setup(
        //        t => t.Execute(It.IsAny<Guid>(), It.IsAny<ILoopCommandData>(), It.IsAny<EventLoopCallback>()))
        //        .Callback<Guid, ILoopCommandData, EventLoopCallback>(
        //            (id, data, waitCallBack) =>
        //            {
        //                waitCallBack(id, EventLoopCommandStatus.Completed, new ResultCommandMessage(new StopCommandResult()));
        //            });
        //    _runner.AddContainter(container.Object);
        //    var commandDefinition = new CommandDefinition(typeof(CommandStub));
        //    const string commandId = "8CC9A7EC-AF69-4EBC-BF2C-072E85212BB1";
        //    var commandKey = new CommandKey(new Guid(commandId));

        //    // ACT
        //    Guid callbackId = Guid.Empty;
        //    var workflow = new CommandWorkflow(_runner);
        //    workflow.MessageRecieved += (id, status, message) =>
        //    {
        //        callbackId = id;
        //        _event.Set();
        //    };

        //    var originalId = _runner.Run(workflow, commandKey, commandDefinition, null);

        //    // ASSERT
        //    _event.WaitOne(1000);
        //    Assert.AreNotEqual(Guid.Empty, callbackId);
        //    Assert.AreEqual(originalId, callbackId);
        //}

        //[TestMethod]
        //public void ContainerCommandExecuteTest()
        //{
        //    // ARRANGE
        //    var container = new SimpleContainer();
        //    _runner.AddContainter(container);
        //    var commandDefinition = new CommandDefinition(typeof(CommandStub));
        //    const string commandId = "8CC9A7EC-AF69-4EBC-BF2C-072E85212BB1";
        //    var commandKey = new CommandKey(new Guid(commandId));

        //    // ACT
        //    object callbackMessage = null;
        //    var workflow = new CommandWorkflow(_runner);
        //    workflow.MessageRecieved += (id, status, message) =>
        //    {
        //        callbackMessage = message;
        //        _event.Set();
        //    };
        //    _runner.Run(workflow, commandKey, commandDefinition, null);

        //    // ASSERT
        //    _event.WaitOne(1000);
        //    Assert.IsNotNull(callbackMessage);
        //    Assert.IsTrue(callbackMessage is DataCommandMessage);
        //    Assert.IsNull((callbackMessage as DataCommandMessage).Data);
        //}


        //[TestMethod]
        //public void PassingInputDataToResultTest()
        //{
        //    // ARRANGE
        //    var container = new SimpleContainer();
        //    _runner.AddContainter(container);
        //    var commandDefinition = new CommandDefinition(typeof(PassingCommand));
        //    const string commandId = "8CC9A7EC-AF69-4EBC-BF2C-072E85212BB1";
        //    var commandKey = new CommandKey(new Guid(commandId));

        //    // ACT
        //    var newId = Guid.NewGuid();
        //    Guid callbackId = Guid.Empty;
        //    var callbackStatus = EventLoopCommandStatus.NotStarted;
        //    ILoopCommandMessage callbackMessage = null;
        //    var workflow = new CommandWorkflow(_runner);
        //    workflow.MessageRecieved += (id, status, message) =>
        //    {
        //        callbackId = id;
        //        callbackStatus = status;
        //        callbackMessage = message;
        //        _event.Set();
        //    };
        //    _runner.Run(workflow, commandKey, commandDefinition, new PassingCommandDataObject(newId));

        //    // ASSERT
        //    _event.WaitOne(1000);
        //    Assert.Fail();
        //    //Assert.AreEqual(originalId, callbackId);
        //    Assert.AreEqual(EventLoopCommandStatus.Completed, callbackStatus);
        //    Assert.IsNotNull(callbackMessage);
        //    Assert.IsTrue(callbackMessage is DataCommandMessage);
        //    var dataMessage = callbackMessage as DataCommandMessage;
        //    Assert.IsNotNull(dataMessage.Data);
        //    Assert.IsTrue(dataMessage.Data is PassingCommandDataObject);
        //    Assert.AreEqual(newId, ((PassingCommandDataObject)dataMessage.Data).Id);
        //}


        //[TestMethod]
        //public void TwoCommandsTest()
        //{
        //    // ARRANGE
        //    var container = new SimpleContainer();
        //    _runner.AddContainter(container);

        //    var definition1 = new CommandDefinition(typeof(Command1));
        //    var definition2 = new CommandDefinition(typeof(Command2));
        //    var сommandWorkflow = new CommandWorkflow(_runner);
        //    сommandWorkflow.Register(definition1);
        //    сommandWorkflow.Register(definition2);
        //    const string commandId1 = "8CC9A7EC-AF69-4EBC-BF2C-072E85212BB1";
        //    var commandKey1 = new CommandKey(new Guid(commandId1));

        //    // ACT
        //    var callbackId = new Guid[2];
        //    var callbackStatus = new EventLoopCommandStatus[2];
        //    var callbackMessage = new ILoopCommandMessage[2];
        //    var callCount = 0;

        //    сommandWorkflow.MessageRecieved += (id, status, message) =>
        //    {
        //        callbackId[callCount] = id;
        //        callbackStatus[callCount] = status;
        //        callbackMessage[callCount] = message;

        //        ++callCount;
        //        if (callCount == 2)
        //            _event.Set();
        //    };

        //    _runner.Run(сommandWorkflow, commandKey1, definition1, new Command1DataObject());

        //    // ASSERT
        //    _event.WaitOne(1000);
        //    Assert.AreEqual(2, callCount);

        //    Assert.AreEqual(EventLoopCommandStatus.Completed, callbackStatus[0]);
        //    Assert.AreEqual(EventLoopCommandStatus.Completed, callbackStatus[1]);

        //    Assert.IsTrue(callbackMessage[0] is NextCommandMessage);
        //    Assert.IsTrue(callbackMessage[1] is DataCommandMessage);
        //    Assert.IsNull((callbackMessage[1] as DataCommandMessage).Data);

        //    Assert.AreNotEqual(Guid.Empty, callbackId[0]);
        //    Assert.AreNotEqual(Guid.Empty, callbackId[1]);

        //    var secondId = (callbackMessage[0] as NextCommandMessage).LoopId;
        //    Assert.Fail();
        //    //Assert.AreEqual(originalId, callbackId[0]);
        //    Assert.AreEqual(secondId, callbackId[1]);

        //}
    }
}
