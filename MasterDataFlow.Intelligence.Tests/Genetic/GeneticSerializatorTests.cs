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
        public class MockGeneticItem : GeneticItem<GeneticItemInitData, double>
        {
            public MockGeneticItem(GeneticItemInitData initData) : base(initData)
            {
            }

            public override double CreateValue(IRandom random)
            {
                return System.Math.Floor(random.NextDouble() * 5);
            }

            public override double ParseStringValue(string value)
            {
                throw new NotImplementedException();
            }

        }

        [TestMethod]
        public void SerializeStopCommandDataObjectIsNullMessageTest()
        {
            // ARRANGE
            var key = new CommandKey();

            var initData = new GeneticItemInitData(10, false);

            var item = new MockGeneticItem(initData);

            var dataObject = new GeneticInfoDataObject
            {
                Best = item
            };

            var message = new StopCommandMessage(key, dataObject);

            //ACT
            var messageString = Serializator.Serialize(message);
            var newMessage = (StopCommandMessage) Serializator.Deserialize(typeof (StopCommandMessage), messageString);

            // ASSERT
            Assert.AreEqual(key.Key, newMessage.Key.Key);
            var newDataOBject = newMessage.Data as GeneticInfoDataObject;
            Assert.IsTrue(newDataOBject.Best is MockGeneticItem);
        }

    }
}
