using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class KeyTests
    {
        [TestMethod]
        public void DeserializeKey()
        {
            // ARRANGE
            var key = new ServiceKey();
            var stringKey = key.Key;

            // ACT
            var deserializedKey = BaseKey.DeserializeKey<ServiceKey>(stringKey);

            // ASSERT
            Assert.AreEqual(stringKey, deserializedKey.Key);
        }
    }
}
