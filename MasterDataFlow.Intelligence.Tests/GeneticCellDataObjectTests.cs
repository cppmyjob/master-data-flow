using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Genetic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Intelligence.Tests
{
    [TestClass]
    public class GeneticCellDataObjectTests
    {
        [TestMethod]
        public void SerializationTest()
        {
            // ASSERT
            var initData = new GeneticInitData(100, 30, 50);
            var value = new GeneticDataObject<double>
            {
                CellInitData = initData
            };


            // ACT
            var serializatedValue = Serialization.Serializator.Serialize(value);
            var newValue = (GeneticDataObject<double>)Serialization.Serializator.Deserialize(typeof(GeneticDataObject<double>), serializatedValue);

            // ARRANGE
            Assert.IsNotNull(newValue.CellInitData);
            Assert.AreEqual(100, newValue.CellInitData.ItemsCount);
            Assert.AreEqual(30, newValue.CellInitData.SurviveCount);
            Assert.AreEqual(50, newValue.CellInitData.ValuesCount);
        }
    }
}
