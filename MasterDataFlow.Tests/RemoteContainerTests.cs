using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Remote;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class RemoteContainerTests
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
        public void ExecuteValidParametersTest()
        {
            // ARRANGE
            Guid requestId = Guid.Empty;
            Guid domainId = Guid.Empty;
            string typeName = null;
            string dataObject = null;
            string dataObjectTypeName = null;
            int calls = 0;

            var contract = new Mock<IRemoteHostContract>();
            contract.Setup(t => t.Execute(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).
                Callback<Guid, Guid, string, string, string>((requestIdParam, domainIdParam, typeNameParem, dataObjectTypeNameParam, dataObjectParam) =>
                {
                    requestId = requestIdParam; 
                    domainId = domainIdParam;
                    typeName = typeNameParem;
                    dataObjectTypeName = dataObjectTypeNameParam;
                    dataObject = dataObjectParam;
                    calls++;
                });
            var container = new RemoteContainer(contract.Object);

            const string guid = "1db907fb-77c7-465f-bd60-031107374727";
            const string domainGuid = "C2B980FF-7C4D-4B43-9935-497218492783";

            var domain = new CommandDomain(new Guid(domainGuid), _runner);

            var info = new CommandInfo
            {
                CommandDefinition = new CommandDefinition(typeof (PassingCommand)),
                CommandDataObject = new PassingCommandDataObject(new Guid(guid)),
                CommandDomain = domain
            };

            // ACT
            container.Execute(Guid.NewGuid(), info, (id, status, message) =>
            {
                _event.Set();
            });

            // ASSERT
            _event.WaitOne(1000);
            Assert.AreEqual(1, calls);
            Assert.AreEqual("MasterDataFlow.Tests.TestData.PassingCommand, MasterDataFlow.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", typeName);
            Assert.AreEqual("MasterDataFlow.Tests.TestData.PassingCommandDataObject, MasterDataFlow.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", dataObjectTypeName);
            Assert.AreEqual("{\"Id\":\"" + guid + "\"}", dataObject);
            Assert.AreEqual(new Guid(domainGuid), domainId);
            Assert.AreNotEqual(Guid.Empty, requestId);
        }

    }
}
