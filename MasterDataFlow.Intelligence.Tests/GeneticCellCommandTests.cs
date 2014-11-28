using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Messages;
using MasterDataFlow.Network;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Intelligence.Tests
{
    [TestClass]
    public class GeneticCellCommandTests
    {
        private static ManualResetEvent _event;

        public class GeneticCellCommandMock : GeneticCellCommand
        {
            protected override BaseMessage BaseExecute()
            {
                _event.Set();
                return base.BaseExecute();
            }
        }
        [TestInitialize]
        public void TestInitialize()
        {
            _event = new ManualResetEvent(false);
        }

        [TestCleanup]
        public void TestCleanup()
        {
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
            var initData = new GeneticCellInitData(100, 30, 50);
            var dataObject = new GeneticCellDataObject
            {
                CellInitData = initData
            };
            сommandWorkflow.Start<GeneticCellCommandMock>(dataObject);

            // ASSERT
            _event.WaitOne(200);
        }
    }
}
