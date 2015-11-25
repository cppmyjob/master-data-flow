using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Intelligence.Neuron.Builder;

namespace MasterDataFlow.Intelligence.Neuron.Dna
{
    public class DnaSection : DnaInOut, IDnaSerializator
    {
        public DnaAtom[] Definitions { get; set; }
        public void Serialize(WriteBuffer buffer)
        {
            
        }
    }
}
