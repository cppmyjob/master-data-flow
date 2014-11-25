using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MasterDataFlow.Tests
{
     [TestClass]
    public class HubTests
    {
         [TestMethod]
         public void ConnectHubTest()
         {
             // ARRANGE
             var key1 = new ServiceKey();
             var hubMock1 = new Mock<Hub> {CallBase = true};
             hubMock1.SetupGet(t => t.Key).Returns(key1);

             var key2 = new ServiceKey();
             var hubMock2 = new Mock<Hub> { CallBase = true };
             hubMock2.SetupGet(t => t.Key).Returns(key2);

             var hub1 = hubMock1.Object;
             var hub2 = hubMock2.Object;

             // ACT
             hub1.ConnectHub(hub2);

             // ASSERT
             Assert.IsTrue(hub1.ConnectedHubs.SingleOrDefault(t => t.Key == hub2.Key) != null);
             Assert.IsTrue(hub2.ConnectedHubs.SingleOrDefault(t => t.Key == hub1.Key) != null);
         }

    }
}
