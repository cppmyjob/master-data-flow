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
        public void IsExecuteWasCalledTestMoq()
        {
            // ARRANGE
            int calls = 0;
            var contract = new Mock<IRemoteHostContract>();
            contract.Setup(t => t.Execute(It.IsAny<string>(), It.IsAny<string>())).Callback(() => calls++);
            var container = new RemoteContainer(contract.Object);
            var info = new CommandInfo();
            var waiter = new ManualResetEvent(false);

            // ACT
            container.Execute(info, (containter, commandInfo) =>
            {
                waiter.Set();
            });

            // ASSERT
            waiter.WaitOne(1000);
            Assert.AreEqual(1, calls);
        }

        [TestMethod]
        public void ExecuteValidParametersTest()
        {
            // ARRANGE
            string typeName = null;
            string dataObject = null;

            var contract = new Mock<IRemoteHostContract>();
            contract.Setup(t => t.Execute(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>(
                (typeNameParem, dataObjectParam) => {
                    typeName = typeNameParem;
                    dataObject = dataObjectParam;
                });
            var container = new RemoteContainer(contract.Object);
            var info = new CommandInfo();
            var waiter = new ManualResetEvent(false);

            // ACT
            container.Execute(info, (containter, commandInfo) =>
            {
                waiter.Set();
            });

            // ASSERT
            waiter.WaitOne(1000);
            Assert.AreEqual("", typeName);
            //Assert.AreEqual("", dataObject);
        }

    }
}
