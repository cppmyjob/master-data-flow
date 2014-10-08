using System;
using MasterDataFlow.Keys;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class CommandDefinitionTests
    {
        [TestMethod]
        public void CommandCreationTest()
        {
            // ARRANGE
            var commandDefinition = new CommandDefinition(typeof(PassingCommand));
            const string commandId = "8CC9A7EC-AF69-4EBC-BF2C-072E85212BB1";
            var commandKey = new CommandKey(new Guid(commandId));

            // ACT
            var command = commandDefinition.CreateInstance(commandKey, new PassingCommandDataObject(Guid.NewGuid()));

            // ASSERT
            Assert.IsNotNull(command);
            Assert.AreEqual(typeof(PassingCommand), commandDefinition.Command);
            Assert.IsTrue(command is PassingCommand);
            Assert.AreEqual(commandKey, command.Key);

        }
    }
}
