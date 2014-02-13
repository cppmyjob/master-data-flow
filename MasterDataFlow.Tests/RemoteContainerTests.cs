using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Remote;
using MasterDataFlow.Tests.Mocks;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class RemoteContainerTests
    {
        private CommandDomainInstance _сommandDomainInstance;
        private CommandDomain _сommandDomain;

        [TestInitialize]
        public void TestInitialize()
        {
            _сommandDomain = new CommandDomain();
            _сommandDomainInstance = new CommandDomainInstance(_сommandDomain);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _сommandDomainInstance.Dispose();
        }

        [TestMethod]
        public void ExecuteValidParametersTest()
        {
            // ARRANGE
            Guid domainId = Guid.Empty;
            string typeName = null;
            string dataObject = null;
            int calls = 0;

            var contract = new Mock<IRemoteHostContract>();
            contract.Setup(t => t.Execute(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).
                Callback<Guid, string, string>((domainIdParam, typeNameParem, dataObjectParam) =>
                {
                    domainId = domainIdParam;
                    typeName = typeNameParem;
                    dataObject = dataObjectParam;
                    calls++;
                });
            var container = new RemoteContainer(contract.Object);
            const string guid = "1DB907FB-77C7-465F-BD60-031107374727";
            const string domainGuid = "C2B980FF-7C4D-4B43-9935-497218492783";
            var info = new CommandInfo
            {
                CommandDefinition = new CommandDefinition(typeof (PassingCommand)),
                CommandDataObject = new PassingCommandDataObject(new Guid(guid)),
                CommandDomainId = new Guid(domainGuid),
            };
            var waiter = new ManualResetEvent(false);

            // ACT
            container.Execute(info, (containter, commandInfo) =>
            {
                waiter.Set();
            });

            // ASSERT
            waiter.WaitOne(1000);
            Assert.AreEqual(1, calls);
            Assert.AreEqual("MasterDataFlow.Tests.TestData.PassingCommand, MasterDataFlow.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", typeName);
            Assert.AreEqual("{\"Id\" : \"" + guid + "\"}", dataObject);
            Assert.AreEqual(new Guid(domainGuid), domainId);
        }

    }
}
