using MasterDataFlow.Intelligence.Interfaces;

namespace MasterDataFlow.Intelligence.Neuron.Dna
{
    public class DnaSection
    {
        public DnaDefinition[] Definitions { get; set; }
        public DnaAxon[] Input { get; set; }
        public int OutputCount { get; set; }
    }
}
