using System;
using MasterDataFlow.Intelligence.Neuron;
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
            var dna = new MasterDna();
            dna.Sections = new[]
                           {
                               new DnaSection
                               {
                                   
                               } 
                           };

            var network = new NetworkInstance(dna);

            // ACT
            network.Compute(new float[] { 12, 13, 14 } );

            // ASSERT

        }
    }
}
