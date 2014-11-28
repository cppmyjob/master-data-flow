using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;
using MasterDataFlow.Network.Packets;
using MasterDataFlow.Network.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class SimpleRoutingTest
    {
        private IPacket _packet;
        private int nonRoutingPacketSent = 0;

        public IPacket Packet
        {
            get { return _packet; }
            set 
            { _packet = value;
                nonRoutingPacketSent++;
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
           _packet = null;
            nonRoutingPacketSent = 0;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            
        }

        [TestMethod]
        public void LineRoutingTest()
        {
            // ARRANGE
            /* Network includes 4 hubs  1 <-> 2 <-> 3 <-> 4. Hubs #1 and #4 didn't connectected directly.
             * Step 1: Route Request 1 -> 2 -> 3 (hub #3 knows about hub #4).
             * Step 2: Route Response 3 -> 2 -> 1 (each hub saves route, so all of them know route between 1 and 4).
             * Step 3: Hub #1 gets route from route table, gets packet from accumulator and sends it to #2.
             * Step 4: Hub #2 gets route from route table and sends packet to #3.
             * Step 5: Hub #3 sends packet to #4.
             * Packet saves info about original sender (#1) and original receiver (#4), so #4 think that packet went from #1 directly.
             */
            var firstNode = new ActionHub();
            var secondNode = new ActionHub();
            var fourthNode = new ActionHub();

            var mock = new Mock<ActionHub>() {CallBase = true};
            mock.Setup(h => h.Send(It.Is<IPacket>(s => s.Body is string))).Callback((IPacket pack) => Packet = pack);
           
            firstNode.ConnectHub(secondNode);

            secondNode.ConnectHub(mock.Object);

            mock.Object.ConnectHub(fourthNode);

            var packet = new Packet(firstNode.Key, fourthNode.Key, "Perfect Routing!");

            // ACT
            firstNode.Send(packet);
            Thread.Sleep(2000);

            // ASSERT
            Assert.AreEqual(nonRoutingPacketSent, 1);
            Assert.AreEqual(Packet.Body is string, true);
            Assert.AreEqual(Packet.Body.ToString(), "Perfect Routing!");
            Assert.AreEqual(Packet.SenderKey, firstNode.Key);
            Assert.AreEqual(Packet.RecieverKey, fourthNode.Key);
        }

        [TestMethod]
        public void VariabilityRoutingTest()
        {
            // ARRANGE
            /* Network includes 4 hubs  1 <-> 2 <-> 4 and 1 <-> 3 <-> 4. Hubs #1 and #4 didn't connectected directly.
             * Step 1: Route Requests 1 -> 2 and 1 -> 3 (hubs #2 and #3 knows about hub #4).
             * Step 2: Route Requests 2 -> 4 and 3 -> 4.
             * Step 3: Route Response 4 -> 2 or 4 -> 3 (it depens on the which request was faster).
             * Step 3: Hub #1 gets route from route table, gets packet from accumulator and sends it to #2 or #3.
             * Step 4: Hub #2 or #3 gets route from route table and sends packet to #4.
             * Packet saves info about original sender (#1) and original receiver (#4), so #4 think that packet went from #1 directly.
             */
            var firstNode = new ActionHub();
            var fourthNode = new ActionHub();

            var secondNodeMock = new Mock<ActionHub>() { CallBase = true };
            secondNodeMock.Setup(h => h.Send(It.Is<IPacket>(s => s.Body is string))).Callback((IPacket pack) => Packet = pack);
            var thirdNodeMock = new Mock<ActionHub>() { CallBase = true };
            thirdNodeMock.Setup(h => h.Send(It.Is<IPacket>(s => s.Body is string))).Callback((IPacket pack) => Packet = pack);

            firstNode.ConnectHub(secondNodeMock.Object);

            firstNode.ConnectHub(thirdNodeMock.Object);

            secondNodeMock.Object.ConnectHub(fourthNode);

            thirdNodeMock.Object.ConnectHub(fourthNode);

            var packet = new Packet(firstNode.Key, fourthNode.Key, "Perfect Routing!");

            // ACT
            firstNode.Send(packet);
            Thread.Sleep(2000);

            // ASSERT
            Assert.AreEqual(nonRoutingPacketSent, 1);
            Assert.AreEqual(Packet.Body is string, true);
            Assert.AreEqual(Packet.Body.ToString(), "Perfect Routing!");
            Assert.AreEqual(Packet.SenderKey, firstNode.Key);
            Assert.AreEqual(Packet.RecieverKey, fourthNode.Key);
        }
    }
}
