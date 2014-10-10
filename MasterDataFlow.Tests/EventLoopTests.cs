using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class EventLoopTests
    {
        //[TestMethod]
        //public void SimpleCallTest()
        //{
        //    // ARRANGE
        //    var loop = new BaseEventLoop();
        //    var command = new Mock<ILoopCommand>();
        //    int callCount = 0;
        //    var paramloopId = new Guid();
        //    command.Setup(t => t.Execute(It.IsAny<Guid>(), It.IsAny<ILoopCommandData>(), It.IsAny<EventLoopCallback>())).Callback<Guid, ILoopCommandData, EventLoopCallback>(
        //        (id, data, waitCallBack) =>
        //        {
        //            paramloopId = id;
        //            ++callCount;
        //        });

        //    var callback = new Mock<EventLoopCallback>();
        //    var loopId = loop.Push(command.Object, callback.Object);

        //    // ACT
        //    loop.Loop();

        //    // ASSERT
        //    Assert.AreEqual(1, callCount);
        //    Assert.AreEqual(loopId, paramloopId);
        //}

        //[TestMethod]
        //public void Loop1PhaseTest()
        //{
        //    // ARRANGE
        //    var loop = new BaseEventLoop();

        //    int commandCallCount = 0;
        //    var command = new Mock<ILoopCommand>();
        //    command.Setup(t => t.Execute(It.IsAny<Guid>(), It.IsAny<ILoopCommandData>(), It.IsAny<EventLoopCallback>())).Callback<Guid, ILoopCommandData, EventLoopCallback>(
        //        (id, data, waitCallBack) =>
        //        {
        //            ++commandCallCount;
        //            waitCallBack(id, EventLoopCommandStatus.Completed);
        //        });

        //    int callbackCallCount = 0;
        //    var loopId = loop.Push(command.Object, (id, status, message) =>
        //    {
        //        ++callbackCallCount;
        //    });

        //    // ACT
        //    loop.Loop();

        //    // ASSERT
        //    Assert.AreEqual(1, commandCallCount);
        //    Assert.AreEqual(0, callbackCallCount);
        //}


        //[TestMethod]
        //public void Loop2PhasesTest()
        //{
        //    // ARRANGE
        //    var loop = new BaseEventLoop();

        //    int commandCallCount = 0;
        //    var command = new Mock<ILoopCommand>();
        //    command.Setup(t => t.Execute(It.IsAny<Guid>(), It.IsAny<ILoopCommandData>(), It.IsAny<EventLoopCallback>())).Callback<Guid, ILoopCommandData, EventLoopCallback>(
        //        (id, data, waitCallBack) =>
        //        {
        //            ++commandCallCount;
        //            waitCallBack(id, EventLoopCommandStatus.Completed);
        //        });

        //    var callbackId = new Guid();
        //    var callbackStatus = EventLoopCommandStatus.NotStarted;
        //    ILoopCommandMessage callbackMessage = null;
        //    int callbackCallCount = 0;
        //    var loopId = loop.Push(command.Object, (id, status, message) =>
        //    {
        //        ++callbackCallCount;
        //        callbackId = id;
        //        callbackStatus = status;
        //        callbackMessage = message;
        //    });

        //    // ACT
        //    loop.Loop();
        //    loop.Loop();

        //    // ASSERT
        //    Assert.AreEqual(1, commandCallCount);
        //    Assert.AreEqual(1, callbackCallCount);
        //    Assert.AreEqual(loopId, callbackId);
        //    Assert.AreEqual(callbackStatus, EventLoopCommandStatus.Completed);
        //    Assert.IsNull(callbackMessage);
        //}

    }
}
