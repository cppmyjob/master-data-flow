using MasterDataFlow.Intelligence.Neuron.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Intelligence.Tests.Builder
{
    [TestClass]
    public class DnaBuilderTests
    {
        [TestMethod]
        public void AddSectionTest()
        {
            // ARRANGE
            var bytes = new byte[] {0x01, 0x03, 0x00};
            var context = new Context();
            var builder = new DnaBuilder(context, bytes);

            // ACT
            var dna = builder.Build();
            
            // ASSERT
            Assert.IsNotNull(dna);
            Assert.IsNotNull(dna.Sections);
            Assert.AreEqual(3, dna.Sections.Length);
        }

        //[TestMethod]
        //public void AddInputsToSectionOperatorTest()
        //{
        //    // ARRANGE
        //    var bytes = new byte[] { 0x01, 0x03,
        //        0x02, // AddInputsToSectionOperator
        //        0x00, // Input index
        //        0x02, // 
        //        0x00 };
        //    var context = new Context();
        //    var builder = new DnaBuilder(context, bytes);

        //    // ACT
        //    var dna = builder.Build();

        //    // ASSERT
        //    Assert.IsNotNull(dna);
        //    Assert.IsNotNull(dna.Sections);
        //    Assert.AreEqual(3, dna.Sections.Length);
        //}

    }
}