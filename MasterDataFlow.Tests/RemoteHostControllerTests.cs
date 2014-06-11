using System;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Remote;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class RemoteHostControllerTests
    {
        private RemoteHost _host;
        private ManualResetEvent _event;

        [TestInitialize]
        public void TestInitialize()
        {
            _host = new RemoteHost();
            _event = new ManualResetEvent(false);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _host.Dispose();
            _event.Dispose();
        }

        [TestMethod]
        public void RemoteHostBasicUsageTest()
        {
            // ARRANGE
            var contract = new Mock<IRemoteCallback>();
            contract.Setup(
                t =>
                    t.Callback(It.IsAny<string>(), It.IsAny<EventLoopCommandStatus>(), It.IsAny<string>(),
                        It.IsAny<string>()))
                .Callback<string, EventLoopCommandStatus, string, string>((loopId, status, messageTypeName, messageData) =>
                {

                });

            var controller = new RemoteHostController(_host, contract.Object);
            var requestId = Guid.NewGuid();
            var domainId = Guid.NewGuid();
            const string commandTypeName = "MasterDataFlow.Tests.TestData.PassingCommand, MasterDataFlow.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            const string dataObjectTypeName = "MasterDataFlow.Tests.TestData.PassingCommandDataObject, MasterDataFlow.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            const string guid = "1db907fb-77c7-465f-bd60-031107374727";
            const string dataObject = "{\"Id\":\"" + guid + "\"}";

            // ACT
            controller.Execute(requestId, domainId, commandTypeName, dataObjectTypeName, dataObject);
            

            // ASSERT
        }
    }
}
