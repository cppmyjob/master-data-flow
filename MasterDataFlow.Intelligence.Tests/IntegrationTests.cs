using System;
using System.Threading;
using MasterDataFlow.Common.Tests;
using MasterDataFlow.Common.Tests.Genetic;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Intelligence.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private RemoteEnvironment _remote;
        private ManualResetEvent _event;

        [TestInitialize]
        public void TestInitialize()
        {
            _remote = new RemoteEnvironment();
            _event = new ManualResetEvent(false);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _remote.Dispose();
            _event.Close();
        }

        [TestMethod]
        public void TestMethod1()
        {
            // ARRANGE

            // ACT
            StopCommandMessage stopMessage = null;
            _remote.CommandWorkflow.MessageRecieved += (key, message) =>
            {
                stopMessage = message as StopCommandMessage;
                if (stopMessage != null)
                    _event.Set();
            };

            var initData = new GeneticCellInitData(1000, 300, 5);
            var dataObject = new OrderDataObject
            {
                CellInitData = initData,
                RepeatCount = 200
            };

            // ASSERT
            _remote.CommandWorkflow.Start<OrderCommand>(dataObject);

            // ASSERT
            _event.WaitOne(5000);

            Assert.IsNotNull(stopMessage);
            Assert.AreEqual(10, ((GeneticItem)(stopMessage.Data as GeneticStopDataObject).Best).Fitness);

        }
    }
}
