using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class CommandDomainTests
    {
        [TestMethod]
        public void IdTests()
        {
            // ARRANGE, ACT
            var domain1 = new CommandDomain();
            var domain2 = new CommandDomain();

            // ASSERT
            Assert.AreNotEqual(Guid.Empty, domain1.Id);
            Assert.AreNotEqual(Guid.Empty, domain2.Id);
            Assert.AreNotEqual(domain1.Id.ToString(), domain2.Id.ToString());
        }
    }
}
