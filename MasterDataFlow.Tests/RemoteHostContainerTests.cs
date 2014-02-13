using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Remote;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class RemoteHostContainerTests
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
        public void FindDomainTest()
        {
            // ARRANGE
            int calls = 0;
            var domainResolver = new Mock<ICommandDomainContainer>();
            domainResolver.Setup(t => t.Find(It.IsAny<Guid>()))
                .Callback(() => ++calls)
                .Returns(() => _сommandDomainInstance);

            var hostContainer = new RemoteHostContainer(domainResolver.Object);

            const string domainId = "C2B980FF-7C4D-4B43-9935-497218492783";
            const string typeName = "MasterDataFlow.Tests.TestData.PassingCommand, MasterDataFlow.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            const string dataObject = "{\"Id\" : \"1DB907FB-77C7-465F-BD60-031107374727\"}";

            // ACT
            hostContainer.Execute(new Guid(domainId), typeName, dataObject);

            // ASSERT
            Assert.AreEqual(1, calls);
        }

        [TestMethod]
        public void CallExecuteDominInstanceTest()
        {
            // ARRANGE
            int calls = 0;
            var сommandDomainInstance = new Mock<ICommandDomainInstance>();
            сommandDomainInstance.Setup(t => t.Start(It.IsAny<Type>(), It.IsAny<ICommandDataObject>()))
                .Callback<Type, ICommandDataObject>(
                    (type, o) =>
                    {
                        ++calls;
                    }).Returns<Type, ICommandDataObject>(null);

            var domainResolver = new Mock<ICommandDomainContainer>();
            domainResolver.Setup(t => t.Find(It.IsAny<Guid>())).Returns(() => сommandDomainInstance.Object);

            var hostContainer = new RemoteHostContainer(domainResolver.Object);

            const string domainId = "C2B980FF-7C4D-4B43-9935-497218492783";
            const string typeName = "MasterDataFlow.Tests.TestData.PassingCommand, MasterDataFlow.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            const string dataObject = "{\"Id\" : \"1DB907FB-77C7-465F-BD60-031107374727\"}";

            // ACT
            hostContainer.Execute(new Guid(domainId), typeName, dataObject);

            // ASSERT
            Assert.AreEqual(1, calls);
        }
    }
}
