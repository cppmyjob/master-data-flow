using MasterDataFlow.Tests._20140530.TestData;
using MasterDataFlow._20140530;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Tests._20140530
{
    [TestClass]
    public class EventLoopTests
    {
        [TestMethod]
        public void SimpleLoop()
        {
            // ARRANGE
            var loop = new EventLoop();
            var command = new CommandStub();
            loop.Push(command);

            // ACT
            loop.Loop();

            // ASSERT
        }
    }
}
