using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Tests.Mocks;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class CommandBuilderTests
    {

        [TestMethod]
        public void SimpleCommandTest() {
            // ARRANGE
            // ACT
            var commandDefinition = CommandBuilder.Build<CommandStub>().Complete();

            // ASSERT
            Assert.AreEqual(typeof(CommandStub), commandDefinition.Command);
        }

    }
}
