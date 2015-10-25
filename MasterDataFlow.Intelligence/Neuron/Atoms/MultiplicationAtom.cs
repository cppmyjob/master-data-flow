using MasterDataFlow.Intelligence.Interfaces;

namespace MasterDataFlow.Intelligence.Neuron.Atoms
{
    public class FloatMultiplicationAtom : INeuronAtom<float>
    {
        public float[] NetworkCompute(float[] inputs)
        {
            if (inputs.Length == 0)
                return new[] { 0F }; 
            float result = inputs[0];
            for (var i = 1; i < inputs.Length; i++)
            {
                result *= inputs[i];
            }
            return new[] { result };
        }
    }

}
