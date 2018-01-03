using MasterDataFlow.Intelligence.Genetic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Intelligence.Tests.Genetic
{
    [TestClass]
    public class GeneticCellDataObjectTests
    {
        [TestMethod]
        public void SerializationTest()
        {
            // ASSERT
            var initItemData = new GeneticItemInitData(50);
            var initData = new GeneticCommandInitData(100, 30, 10);
            var value = new GeneticDataObject<GeneticItemInitData, GeneticDoubleItem, double>
            {
                ItemInitData = initItemData,
                CommandInitData = initData
            };


            // ACT
            var serializatedValue = Serialization.Serializator.Serialize(value);
            var newValue = (GeneticDataObject<GeneticItemInitData, GeneticDoubleItem, double>)Serialization.Serializator.Deserialize(typeof(GeneticDataObject<GeneticItemInitData, GeneticDoubleItem, double>), serializatedValue);

            // ARRANGE
            Assert.IsNotNull(newValue.CommandInitData);
            Assert.AreEqual(100, newValue.CommandInitData.ItemsCount);
            Assert.AreEqual(30, newValue.CommandInitData.SurviveCount);
            Assert.AreEqual(10, newValue.CommandInitData.RepeatCount);
            Assert.AreEqual(50, newValue.ItemInitData.ValuesNumber);
        }
    }
}
