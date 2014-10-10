using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;
using MasterDataFlow.Remote;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class RemoteClientContextTests
    {
        private const string LoopId = "1db907fb-77c7-465f-bd60-031107374727";


        private class RemoteClientContextMock : RemoteClientContext
        {
            private readonly IRemoteHostContract _contract;

            public RemoteClientContextMock(IRemoteHostContract contract)
            {
                _contract = contract;
            }

            protected override IRemoteHostContract CreateContract()
            {
                return _contract;
            }
        }

        // TODO Restore
        //[TestMethod]
        //public void ExecuteValidRemoteCallbackTest()
        //{
        //    // ARRANGE
        //    var context = new RemoteClientContextMock(null);
 
        //    // ACT
        //    int calls = 0;
        //    Guid? callbackId = null;
        //    var callbackStatus = EventLoopCommandStatus.NotStarted;
        //    ILoopCommandMessage callbackMessage = null;
        //    context.RegisterCallback(new Guid(LoopId), (id, status, message) =>
        //    {
        //        ++calls;
        //        callbackId = id;
        //        callbackStatus = status;
        //        callbackMessage = message;
        //    });


        //    context.Callback(LoopId, EventLoopCommandStatus.Fault,
        //        "MasterDataFlow.Messages.FaultCommandMessage, MasterDataFlow, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
        //        "{\"Exception\":{\"ClassName\":\"System.Exception\",\"Message\":\"Test\",\"Data\":null,\"InnerException\":null,\"HelpURL\":null,\"StackTraceString\":null,\"RemoteStackTraceString\":null,\"RemoteStackIndex\":0,\"ExceptionMethod\":null,\"HResult\":-2146233088,\"Source\":null,\"WatsonBuckets\":null}}");

        //    // ASSERT
        //    Assert.AreEqual(1, calls);
        //    Assert.AreEqual(EventLoopCommandStatus.Fault, callbackStatus);
        //    Assert.IsTrue(callbackMessage is FaultCommandMessage);
        //    Assert.AreEqual("Test", ((FaultCommandMessage)callbackMessage).Exception.Message);
        //}
         
         
    }
}
