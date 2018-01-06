using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDataFlow.Intelligence.Interfaces
{
    public interface ISimpleNeuron
    {
        void NetworkCreate(float alpha, int[] neuronCount);
        float[] NetworkCompute(float[] inputs);
        void SetAlpha(float alpha);
        float[] GetWeigths();
        void CreateWeigths(int length);
    }
}
