using MasterDataFlow.Intelligence.Neuron.Dna;

namespace MasterDataFlow.Intelligence.Neuron
{
    public class NetworkInstance
    {
        private readonly MasterDna _dna;

        public NetworkInstance(MasterDna dna)
        {
            _dna = dna;
        }

        public float[] Compute(float[] input)
        {
            return new[] { 0F };
        }
    }
}
