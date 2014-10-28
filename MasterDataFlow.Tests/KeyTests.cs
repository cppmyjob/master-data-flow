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
        public void DeserializeServiceKey()
        {
            // ARRANGE
            var key = new ServiceKey();
            var stringKey = key.Key;

            // ACT
            var deserializedKey = BaseKey.DeserializeKey(stringKey);

            // ASSERT
            Assert.AreEqual(typeof(ServiceKey), deserializedKey.GetType());
            Assert.AreEqual(stringKey, deserializedKey.Key);
        }

        [TestMethod]
        public void DeserializeWorkflowKey()
        {
            // ARRANGE
            var key = new WorkflowKey();
            var stringKey = key.Key;

            // ACT
            var deserializedKey = BaseKey.DeserializeKey(stringKey);

            // ASSERT
            Assert.AreEqual(typeof(WorkflowKey), deserializedKey.GetType());
            Assert.AreEqual(stringKey, deserializedKey.Key);
        }

        [TestMethod]
        public void DeserializeCommandKey()
        {
            // ARRANGE
            var key = new CommandKey();
            var stringKey = key.Key;

            // ACT
            var deserializedKey = BaseKey.DeserializeKey(stringKey);

            // ASSERT
            Assert.AreEqual(typeof(CommandKey), deserializedKey.GetType());
            Assert.AreEqual(stringKey, deserializedKey.Key);
        }

    }
}
