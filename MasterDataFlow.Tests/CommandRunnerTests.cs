using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Tests.Mocks;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class CommandRunnerTests
    {
        [TestMethod]
        public void ContainerExecutedTest()
        {
            // ARRANGE
            var commandRunner = new CommandRunner();
            var container = new MockContainer();
            commandRunner.AddContainter(container);
            var commandDefinition = new CommandDefinition(typeof (CommandStub));
            var waiter = new ManualResetEvent(false);

            // ACT
            commandRunner.Run(null, commandDefinition, null, (status, exception, dataObject) => waiter.Set());

            // ASSERT
            waiter.WaitOne(1000);
            Assert.IsTrue(container.IsExecuted);
        }
    }
}
