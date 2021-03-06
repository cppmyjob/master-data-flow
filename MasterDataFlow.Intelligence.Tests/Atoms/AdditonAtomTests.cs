﻿using System;
using MasterDataFlow.Intelligence.Neuron.Atoms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Intelligence.Tests.Atoms
{
    [TestClass]
    public class AdditonAtomTests
    {
        [TestMethod]
        public void SimpleUsage()
        {
            // ARRANGE
            var atom = new FloatAdditionAtom();

            // ACT
            var value = atom.NetworkCompute(new[] {10.5F, 9.5F, 12F});

            // ASSERT
            Assert.IsNotNull(value);
            Assert.AreEqual(1, value.Length);
            Assert.AreEqual(32F, value[0]);
        }
    }
}
