using System;
using MasterDataFlow.Intelligence.Neuron;
using MasterDataFlow.Intelligence.Neuron.Atoms.Dna;
using MasterDataFlow.Intelligence.Neuron.Dna;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Intelligence.Tests.Neuron
{
    [TestClass]
    public class NetworkTests
    {
        [TestMethod]
        public void Compute()
        {
            // ARRANGE
            var dna = CreateSimpleDna();

            var network = new NetworkInstance(dna);

            // ACT
            network.Compute(new float[] {12, 13, 14});

            // ASSERT

        }

        [TestMethod]
        public void MoveInputValuesTest()
        {
            // ARRANGE
            var dna = CreateSimpleDna();

            var network = new NetworkInstance(dna);

            // ACT
            network.Compute(new float[] { 12, 13 });

            // ASSERT
            Assert.AreEqual(12, network.Sections[0].Inputs[0].Value);
            Assert.AreEqual(13, network.Sections[0].Inputs[1].Value);
        }

        private static MasterDna CreateSimpleDna()
        {
            var dna = new MasterDna();
            dna.Inputs = new[]
            {
                new DnaAxon(0, new[] {0}),
                new DnaAxon(1, new[] {1})
            };
            dna.Outputs = new[]
            {
                new DnaAxon(2, new int[0]),
            };

            dna.Sections = new[]
            {
                new DnaSection
                {
                    Inputs = new[]
                    {
                        new DnaAxon(0, new[] {0}),
                        new DnaAxon(1, new[] {1})
                    },
                    Outputs = new[]
                    {
                        new DnaAxon(2, new int[] {2}),
                    },
                    Definitions = new[]
                    {
                        new AdditionAtomDefinitions
                        {
                            Inputs = new[] {0, 1},
                            Outputs = new DnaAxon[]
                            {
                                new DnaAxon(2, new int[] {2}),
                            }
                        }
                    }
                },
            };
            return dna;
        }
    }
}
