using System;
using System.Collections.Generic;
using MasterDataFlow.Intelligence.Interfaces;

namespace MasterDataFlow.Intelligence.Neuron.SimpleNeuron
{
    public class GeneiticNeuron
    {
        public int WeightsLength;
        public int WeightsOffset;
        public int layerIndex;
        public float[] Inputs;
        public float[] Outputs;
    }

    public class GeneticNetwork
    {
        public GeneticLayer[] Layers;
    }

    public class GeneticLayer
    {
        public GeneiticNeuron[] Neurons;
        public float[] Outputs;
    }

    public class GeneticQueue
    {
        public GeneiticNeuron[] Neurons;
    }

    public class Neuron : ISimpleNeuron
    {
        private GeneticNetwork _network;
        private GeneticQueue _queue;
        private float[] _weights;
        private float _alpha;

        private GeneticQueue QueueCreateFromNetwork(GeneticNetwork network)
        {
            List<GeneiticNeuron> list = new List<GeneiticNeuron>();
            for (int i = 1; i < network.Layers.Length; i++)
            {
                GeneticLayer layer = network.Layers[i];
                for (int j = 0; j < layer.Neurons.Length; j++)
                {
                    list.Add(layer.Neurons[j]);
                }
            }
            GeneticQueue queue = new GeneticQueue();
            queue.Neurons = list.ToArray();
            return queue;
        }


        public void NetworkCreate(float alpha, int[] neuronCount)
        {
            _alpha = alpha;
            _network = new GeneticNetwork();
            if (neuronCount == null || neuronCount.Length < 2)
                throw new Exception("Должно быть как минимум 2 элемента массива для количества нейронов входа и выхода");

            int weigthsOffset = 0;
            List<GeneticLayer> layersList = new List<GeneticLayer>();
            for (int i = 0; i < neuronCount.Length; i++)
            {
                GeneticLayer layer = new GeneticLayer();
                layersList.Add(layer);
                layer.Outputs = new float[neuronCount[i]];
                // 1-ый слой - только входа
                if (i != 0)
                {
                    List<GeneiticNeuron> layerNeuronsList = new List<GeneiticNeuron>();
                    for (int j = 0; j < neuronCount[i]; j++)
                    {
                        GeneiticNeuron neuron = new GeneiticNeuron();
                        neuron.WeightsLength = neuronCount[i - 1];
                        neuron.WeightsOffset = weigthsOffset;
                        weigthsOffset += neuron.WeightsLength;
                        neuron.layerIndex = j;
                        neuron.Inputs = layersList[i - 1].Outputs;
                        neuron.Outputs = layer.Outputs;
                        layerNeuronsList.Add(neuron);
                    }
                    layer.Neurons = layerNeuronsList.ToArray();
                }
            }

            _network.Layers = layersList.ToArray();

            _queue = QueueCreateFromNetwork(_network);
        }


        public float[] NetworkCompute(float[] inputs)
        {
            if (inputs.Length != _network.Layers[0].Outputs.Length)
                throw new Exception("Несовпадает количество входов сети в количеством входных значений");
            // Заполняет входные данные сети
            GeneticLayer firstLayer = _network.Layers[0];
            for (int i = 0; i < inputs.Length; i++)
            {
                firstLayer.Outputs[i] = inputs[i];
            }
            QueueProcess(_queue);

            return _network.Layers[_network.Layers.Length - 1].Outputs;
        }


        private static void NeuronCalculate(GeneiticNeuron neuron, float[] weights, float alpha)
        {
            //float[] inputs = neuron.Inputs;
            //int neuronNeuronsLength = inputs.Length;
            //int offset = neuron.WeightsOffset;
            //float sum = 0.0;
            //for (int i = 0; i < neuronNeuronsLength; i++)
            //{
            //    sum += inputs[i] * weights[offset + i];
            //}
            //neuron.Outputs[neuron.layerIndex] = 1.0 / (1.0 + Math.Exp(-(alpha * sum)));


            float sum = 0.0F;
            for (int i = 0; i < neuron.WeightsLength; i++)
            {
                sum += neuron.Inputs[i] * weights[neuron.WeightsOffset + i];
            }
            neuron.Outputs[neuron.layerIndex] = (float)(1.0 / (1.0 + global::System.Math.Exp(-(alpha * sum))));
        }

        private void QueueProcess(GeneticQueue queue)
        {
            int queueNeuronsLength = queue.Neurons.Length;
            for (int i = 0; i < queueNeuronsLength; i++)
            {
                GeneiticNeuron neuron = queue.Neurons[i];
                NeuronCalculate(neuron, _weights, _alpha);
            }
        }



        //private static void QueueProcess(GeneticQueue queue, float[] weights, float alpha)
        //{
        //    int queueNeuronsLength = queue.Neurons.Length;
        //    for (int i = 0; i < queueNeuronsLength; i++)
        //    {
        //        GeneiticNeuron neuron = queue.Neurons[i];
        //        int neuronNeuronsLength = neuron.Neurons.Length;
        //        GeneiticNeuron[] neurons = neuron.Neurons;
        //        int offset = neuron.WeightsOffset;
        //        float sum = 0.0;
        //        for (int j = 0; j < neuronNeuronsLength; j++)
        //        {
        //            float input = neurons[j].Output;
        //            sum = sum + input * weights[offset + j];
        //        }
        //        neuron.Output = 1.0 / (1.0 + Math.Exp(-(alpha * sum)));
        //    }
        //}



        // Function

        //private static float MathSigmoid(float value)
        //{
        //    //return 1.0 / (1.0 + (float)Math.Exp(-value));
        //    return 1.0 / (1.0 + Math.Exp(-value));
        //}

        public void SetAlpha(float alpha)
        {
            _alpha = alpha;
        }

        //public void SetWeigths(float[] weights, int offset)
        //{
        //    for (int i = 0; i < _queue.Neurons.Length; i++)
        //    {
        //        GeneiticNeuron neuron = _queue.Neurons[i];
        //        //neuron.Weights = weights;
        //        neuron.WeightsOffset = offset;
        //        offset += neuron.WeightsLength;
        //    }
        //}

        public float[] GetWeigths()
        {
            return _weights;
        }

        public void CreateWeigths(int length)
        {
            _weights = new float[length];
            //            int offset = 0;
            //            for (int i = 0; i < _queue.Neurons.Length; i++)
            //            {
            //                GeneiticNeuron1 neuron = _queue.Neurons[i];
            ////                neuron.Weights = _weights;
            //                neuron.WeightsOffset = offset;
            //                offset += neuron.WeightsLength;
            //            }
        }



        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion


    }

}
