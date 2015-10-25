using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Neuron.Atoms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Intelligence.Tests.Atoms
{
    [TestClass]
    public class MultiplicationAtomTests
    {
        [TestMethod]
        public void SimpleUsage()
        {
            // ARRANGE
            var atom = new FloatMultiplicationAtom();

            // ACT
            var value = atom.NetworkCompute(new[] { 10.5F, 9.5F, 12F });

            // ASSERT
            Assert.IsNotNull(value);
            Assert.AreEqual(1, value.Length);
            Assert.AreEqual(1197F, value[0]);
        }

        [TestMethod]
        public void EmptyUsage()
        {
            // ARRANGE
            var atom = new FloatMultiplicationAtom();

            // ACT
            var value = atom.NetworkCompute(new float[0]);

            // ASSERT
            Assert.IsNotNull(value);
            Assert.AreEqual(1, value.Length);
            Assert.AreEqual(0F, value[0]);
        }

    }
}
