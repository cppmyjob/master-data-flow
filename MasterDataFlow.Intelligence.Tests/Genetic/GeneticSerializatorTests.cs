using System;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Intelligence.Tests.Genetic
{
    [TestClass]
    public class GeneticSerializatorTests
    {
        [Serializable]
        public class MockGeneticItem : GeneticItem<double>
        {
            public MockGeneticItem(GeneticItemInitData initData) : base(initData)
            {
            }

            public override double CreateValue(IRandom random)
            {
                return Math.Floor(random.NextDouble() * 5);
            }
        }

        [TestMethod]
        public void SerializeStopCommandDataObjectIsNullMessageTest()
        {
            // ARRANGE
            var key = new CommandKey();

            var initData = new GeneticItemInitData
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
