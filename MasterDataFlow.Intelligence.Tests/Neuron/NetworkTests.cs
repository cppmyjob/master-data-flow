using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Neuron;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Intelligence.Tests.Neuron
{
    [TestClass]
    public class NetworkTests
    {
        [TestMethod]
        public void NetworkCreateTest()
        {
            // ARRANGE
            var network = new FloatNeuronNetwork();

            // ACT
            network.NetworkCreate(3F, new int[] { 10, 20, 30 });

            // ASSERT
            Assert.AreEqual(2, network.NetworkGetLayersCount());
            Assert.AreEqual(10, network.NetworkGetInputCount());
            Assert.AreEqual(30, network.NetworkGetOutputCount());
        }

        [TestMethod]
        public void NetworkSetGetNeuronWeigths()
        {
            // ARRANGE
            var network = new FloatNeuronNetwork();

            float[] weigths0_0 = new float[] { 0.1F, 0.2F, 0.3F };
            float[] weigths0_1 = new float[] { 0.4F, 0.5F, 0.6F };
            float[] weigths1_0 = new float[] { 0.7F, 0.8F };

            network.NetworkCreate(3F, new int[] { 3, 2, 1 });

            // ACT
            network.NetworkSetNeuronWeights(0, 0, weigths0_0);
            network.NetworkSetNeuronWeights(0, 1, weigths0_1);
            network.NetworkSetNeuronWeights(1, 0, weigths1_0);

            float[] getWeigths0_0 = network.NetworkGetNeuronWeights(0, 0);
            float[] getWeigths0_1 = network.NetworkGetNeuronWeights(0, 1);
            float[] getWeigths1_0 = network.NetworkGetNeuronWeights(1, 0);

            // ASSERT

            Assert.AreEqual(weigths0_0[0], getWeigths0_0[0]);
            Assert.AreEqual(weigths0_0[1], getWeigths0_0[1]);
            Assert.AreEqual(weigths0_0[2], getWeigths0_0[2]);

            Assert.AreEqual(weigths0_1[0], getWeigths0_1[0]);
            Assert.AreEqual(weigths0_1[1], getWeigths0_1[1]);
            Assert.AreEqual(weigths0_1[2], getWeigths0_1[2]);

            Assert.AreEqual(weigths1_0[0], getWeigths1_0[0]);
            Assert.AreEqual(weigths1_0[1], getWeigths1_0[1]);

        }

        [TestMethod]
        public void NetworkCompute()
        {
            // INIT
            var network = new FloatNeuronNetwork();
            float[] weigths0_0 = new float[] { 0.11F, 0.12F, 0.13F };
            float[] weigths0_1 = new float[] { -0.4F, -0.5F, -0.6F };
            float[] weigths1_0 = new float[] { 0.71F, 0.84F };
            network.NetworkCreate(3F, new int[] { 3, 2, 1 });
            network.NetworkSetNeuronWeights(0, 0, weigths0_0);
            network.NetworkSetNeuronWeights(0, 1, weigths0_1);
            network.NetworkSetNeuronWeights(1, 0, weigths1_0);

            // ACT
            float[] result = network.NetworkCompute(new float[] { 0.1F, 0.55F, 0.78F });

            // ASSERT
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(0.826781511306763F, result[0]);
        }


    }
}
