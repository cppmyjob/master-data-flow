using System;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Network.Routing;
using MasterDataFlow.Serialization;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class SerializatorTest
    {
        [TestMethod]
        public void  SerializeRouteTest()
        {
            // ARRANGE
            BaseKey destinationKey = new ServiceKey();
            BaseKey gateKey = new ServiceKey();
            const uint length = 44;
            var route = new Route(destinationKey, gateKey, length);

            //ACT

            var routeString = Serializator.Serialize(route);
            var newRoute = (Route)Serializator.Deserialize(typeof (Route), routeString);

            // ASSERT
            Assert.AreEqual(length, newRoute.Length);
            Assert.AreEqual(destinationKey.Key, newRoute.Destination.Key);
            Assert.AreEqual(gateKey.Key, newRoute.Gate.Key);
        }

        [TestMethod]
        public void SerializeStopCommandMessageTest()
        {
            // ARRANGE
            var key = new CommandKey();
            var dataObject = new PassingCommandDataObject(Guid.NewGuid());
            var message = new StopCommandMessage(key, dataObject);

            //ACT
            var messageString = Serializator.Serialize(message);
            var newMessage = (StopCommandMessage)Serializator.Deserialize(typeof(StopCommandMessage), messageString);

            // ASSERT
            Assert.AreEqual(key.Key, newMessage.Key.Key);
            Assert.AreEqual(dataObject.Id, ((PassingCommandDataObject)newMessage.Data).Id);
        }

        [TestMethod]
        public void SerializeStopCommandDataObjectIsNullMessageTest()
        {
            // ARRANGE
            var key = new CommandKey();
            var message = new StopCommandMessage(key, null);

            //ACT
            var messageString = Serializator.Serialize(message);
            var newMessage = (StopCommandMessage)Serializator.Deserialize(typeof(StopCommandMessage), messageString);

            // ASSERT
            Assert.AreEqual(key.Key, newMessage.Key.Key);
        }

        [TestMethod]
        public void CrossDomainSerializationTest()
        {
            // ARRANGE
            var key = new CommandKey();
            var dataObject = new PassingCommandDataObject(Guid.NewGuid());
            var message = new StopCommandMessage(key, dataObject);

            //ACT
            var messageString = Serializator.Serialize(message);


            var newMessage = (StopCommandMessage)Serializator.Deserialize(typeof(StopCommandMessage), messageString);

            // ASSERT
            Assert.AreEqual(key.Key, newMessage.Key.Key);
            Assert.AreEqual(dataObject.Id, ((PassingCommandDataObject)newMessage.Data).Id);
        }

    }
}
