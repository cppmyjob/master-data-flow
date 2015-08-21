using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Network.Routing;
using MasterDataFlow.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Intelligence.Tests
{
    [TestClass]
    public class GeneticSerializatorTest
    {
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

        [TestMethod]
        public void SerializeStopCommandDataObjectIsNullMessageTest()
        {
            // ARRANGE
            var key = new CommandKey();

            var initData = new GeneticInitData
            {
                Count = 10,
                YearOfBorn = 3
            };

            var item = new MockGeneticItem(initData);

            var dataObject = new GeneticStopDataObject
            {
                Best = item
            };

            var message = new StopCommandMessage(key, dataObject);

            //ACT
            var messageString = Serializator.Serialize(message);
            var newMessage = (StopCommandMessage) Serializator.Deserialize(typeof (StopCommandMessage), messageString);

            // ASSERT
            Assert.AreEqual(key.Key, newMessage.Key.Key);
            var newDataOBject = newMessage.Data as GeneticStopDataObject;
            Assert.IsTrue(newDataOBject.Best is MockGeneticItem);
        }

    }
}
