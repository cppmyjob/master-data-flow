using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private static ManualResetEvent _event;
        private static GeneticCellDataObject _dataObject;
        private static StopCommandMessage _stopMessage;

        [Serializable]
        public class MockGeneticItem : GeneticItem
        {
            public MockGeneticItem(GeneticInitData initData) : base(initData)
            {
            }

            protected override double CreateValue(double random)
            {
                return Math.Floor(random * 5);
            }
        }

        [Serializable]
        public class GeneticCellDataObjectMock : GeneticCellDataObject
        {
        }

        public class GeneticCellCommandMock : GeneticCellCommand
        {
            protected override BaseMessage BaseExecute()
            {
                _dataObject = DataObject;
                Console.WriteLine("GeneticCellCommandMock::BaseExecute");
                return base.BaseExecute();
            }

            protected override GeneticItem CreateItem(GeneticInitData initData)
            {
                return new MockGeneticItem(initData);
            }

            public override double CalculateFitness(GeneticItem item, int processor)
            {
                var result = 1;
                var lastValue = item.Values[0];
                for (int i = 1; i < item.Values.Length; i++)
                {
                    if (lastValue < item.Values[i])
                    {
                        lastValue = item.Values[i];
                        result += 1;
                    }
                    else
                        break;
                }

                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < item.Values.Length; j++)
                    {
                        if (i == item.Values[j])
                        {
                            result += 1;
                            break;
                        }
                    }
                    
                }
                return result;
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
            _dataObject = null;
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

            сommandWorkflow.MessageRecieved += (key, message) =>
            {
                _stopMessage = message as StopCommandMessage;
                if (_stopMessage != null)
                    _event.Set();
            };

            var initData = new GeneticCellInitData(1000, 300, 5);
            var dataObject = new GeneticCellDataObject
            {
                CellInitData = initData,
                RepeatCount = 200
            };
            сommandWorkflow.Start<GeneticCellCommandMock>(dataObject);

            // ASSERT
            _event.WaitOne(20000);
            Assert.IsNotNull(_dataObject);
            //Assert.AreEqual(100, _dataObject.CellInitData.ItemsCount);
            //Assert.AreEqual(30, _dataObject.CellInitData.SurviveCount);
            //Assert.AreEqual(10, _dataObject.CellInitData.ValuesCount);

            Assert.IsNotNull(_stopMessage);
            Assert.AreEqual(10, ((GeneticItem)(_stopMessage.Data as GeneticStopDataObject).Best).Fitness);

        }


        [TestMethod]
        public void GeneticCellInitDataPassingTest()
        {
            // ARRANGE
            var remote = new RemoteEvironment();
            var сommandWorkflow = remote.PrepeareSingleRemoteContainer();

            // ACT
            сommandWorkflow.MessageRecieved += (key, message) =>
            {
                _stopMessage = message as StopCommandMessage;
                if (_stopMessage != null)
                    _event.Set();
            };

            var initData = new GeneticCellInitData(1000, 300, 5);
            var dataObject = new GeneticCellDataObjectMock
            {
                CellInitData = initData,
                RepeatCount = 200
            };

            // ASSERT
            сommandWorkflow.Start<GeneticCellCommandMock>(dataObject);

            // ASSERT
            _event.WaitOne(20000);

            Assert.IsNotNull(_stopMessage);
            Assert.AreEqual(10, ((GeneticItem)(_stopMessage.Data as GeneticStopDataObject).Best).Fitness);


        }
    }
}
