using System;

namespace MasterDataFlow.Intelligence.Neuron.Dna
{
    public class DnaAtomDefinition
    {
        public string AtomData { get; set; }
        public Type AtomType { get; set; }
        public int[] Inputs { get; set; }
        public DnaAxon[] Outputs { get; set; }
    }
}
