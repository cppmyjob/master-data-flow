using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Interfaces;

namespace MasterDataFlow.Intelligence.Atoms
{
    public class FloatAddAtom : INeuronAtom<float>
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
