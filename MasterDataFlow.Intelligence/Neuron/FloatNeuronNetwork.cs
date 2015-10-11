using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Neuron
{
    public class FloatNeuronNetwork : NeuronNetwork<float>
    {
        protected override float Substract(float value1, float value2)
        {
            return value1 - value2;
        }

        protected override float Multiple(float value1, float value2, float value3)
        {
            return value1 * value2 * value3;
        }

        protected override float Multiple(float value1, float value2)
        {
            return value1 * value2;
        }

        protected override float Add(float value1, float value2)
        {
            return value1 + value2;
        }


        protected override float Substract(int value1, float value2)
        {
            return value1 - value2;
        }

        protected override float Add(int value1, float value2)
        {
            return value1 + value2;
        }

        protected override float Divide(float value1, int value2)
        {
            return value1 / value2;
        }

        protected override double Negative(float value)
        {
            return -value;
        }

        protected override float ToValue(double value)
        {
            return (float)value;
        }
    }
}
