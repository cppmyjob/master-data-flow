using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Common.Tests;
using MasterDataFlow.Common.Tests.Genetic;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Network;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Intelligence.Tests
{
    [TestClass]
    public class GeneticCellCommandTests
    {
        private ManualResetEvent _event;
        private RemoteEnvironment _remote; 

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
            OrderCommand.StaticDataObject = null;
            _event.Close();
        }

        [TestMethod]
        public void SimpleCommandExecuteTest()
        {
            // ARRANGE
            var container = new SimpleContainer();
            var runner = new CommandRunner();
            runner.ConnectHub(container);

            var сommandWorkflow = new CommandWorkflow();
            runner.ConnectHub(сommandWorkflow);

            // ACT
            StopCommandMessage stopMessage = null;
            сommandWorkflow.MessageRecieved += (key, message) =>
            {
                stopMessage = message as StopCommandMessage;
                if (stopMessage != null)
                    _event.Set();
            };

            var initData = new GeneticInitData(1000, 300, 5);
            var dataObject = new OrderDataObject
            {
                CellInitData = initData,
                RepeatCount = 200
            };
            сommandWorkflow.Start<OrderCommand>(dataObject);

            // ASSERT
            _event.WaitOne(5000);
            Assert.IsNotNull(OrderCommand.StaticDataObject);
            Assert.AreEqual(1000, OrderCommand.StaticDataObject.CellInitData.ItemsCount);
            Assert.AreEqual(300, OrderCommand.StaticDataObject.CellInitData.SurviveCount);
            Assert.AreEqual(5, OrderCommand.StaticDataObject.CellInitData.ValuesCount);

            Assert.IsNotNull(stopMessage);
            Assert.AreEqual(10, ((GeneticItem<double>)(stopMessage.Data as GeneticStopDataObject).Best).Fitness);

        }

        [TestMethod]
        public void GeneticCellInitDataPassingTest()
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

            var initData = new GeneticInitData(1000, 300, 5);
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
            Assert.AreEqual(10, ((GeneticItem<double>)(stopMessage.Data as GeneticStopDataObject).Best).Fitness);


        }
    }
}
