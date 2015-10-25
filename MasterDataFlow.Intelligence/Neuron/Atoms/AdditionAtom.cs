using MasterDataFlow.Intelligence.Interfaces;

namespace MasterDataFlow.Intelligence.Neuron.Atoms
{
    public class FloatAdditionAtom : INeuronAtom<float>
    {
        public float[] NetworkCompute(float[] inputs)
        {
            float result = 0;
            for (var i = 0; i < inputs.Length; i++)
            {
                result += inputs[i];
            }
            return new []{ result };
        }
    }
}
