using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Remote;
using MasterDataFlow.Tests.Mocks;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        //[TestMethod]
        //public void BasicUsageTest()
        //{
        //    // ARRANGE
        //    var contract = new MockRemoteHostContract();
        //    var container = new RemoteContainer(contract);
        //    var info = new CommandInfo();
        //    var waiter = new ManualResetEvent(false);

        //    // ACT
        //    container.Execute(info, (containter, commandInfo) =>
        //    {
        //        waiter.Set();
        //    });

        //    // ASSERT
        //    waiter.WaitOne(1000);
        //    //Assert.IsTrue(container.IsExecuted);

        //}

    }
}
