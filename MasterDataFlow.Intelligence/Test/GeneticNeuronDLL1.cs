using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Test
{
    public class GeneiticNeuron1
    {
        public int WeightsLength;
        public int WeightsOffset;
        public int layerIndex;
        public double[] Inputs;
        public double[] Outputs;
    }

    public class GeneticNetwork1
    {
        public GeneticLayer1[] Layers;
    }

    public class GeneticLayer1
    {
        public GeneiticNeuron1[] Neurons;
        public double[] Outputs;
    }

    public class GeneticQueue1
    {
        public GeneiticNeuron1[] Neurons;
    }

    public class GeneticNeuronDLL1// : INeuronDLL
    {
        private GeneticNetwork1 _network;
        private GeneticQueue1 _queue;
        private double[] _weights;
        private double _alpha;

        private GeneticQueue1 QueueCreateFromNetwork(GeneticNetwork1 network)
        {
            List<GeneiticNeuron1> list = new List<GeneiticNeuron1>();
            for (int i = 1; i < network.Layers.Length; i++)
            {
                GeneticLayer1 layer = network.Layers[i];
                for (int j = 0; j < layer.Neurons.Length; j++)
                {
                    list.Add(layer.Neurons[j]);
                }
            }
            GeneticQueue1 queue = new GeneticQueue1();
            queue.Neurons = list.ToArray();
            return queue;
        }


        public void NetworkCreate(double alpha, int[] neuronCount)
        {
            _alpha = alpha;
            _network = new GeneticNetwork1();
            if (neuronCount == null || neuronCount.Length < 2)
                throw new Exception("Должно быть как минимум 2 элемента массива для количества нейронов входа и выхода");

            int weigthsOffset = 0;
            List<GeneticLayer1> layersList = new List<GeneticLayer1>();
            for (int i = 0; i < neuronCount.Length; i++)
            {
                GeneticLayer1 layer = new GeneticLayer1();
                layersList.Add(layer);
                layer.Outputs = new double[neuronCount[i]];
                // 1-ый слой - только входа
                if (i != 0)
                {
                    List<GeneiticNeuron1> layerNeuronsList = new List<GeneiticNeuron1>();
                    for (int j = 0; j < neuronCount[i]; j++)
                    {
                        GeneiticNeuron1 neuron = new GeneiticNeuron1();
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


        public double[] NetworkCompute(double[] inputs)
        {
            if (inputs.Length != _network.Layers[0].Outputs.Length)
                throw new Exception("Несовпадает количество входов сети в количеством входных значений");
            // Заполняет входные данные сети
            GeneticLayer1 firstLayer = _network.Layers[0];
            for (int i = 0; i < inputs.Length; i++)
            {
                firstLayer.Outputs[i] = inputs[i];
            }
            QueueProcess(_queue);

            return _network.Layers[_network.Layers.Length - 1].Outputs;
        }


        private static void NeuronCalculate(GeneiticNeuron1 neuron, double[] weights, double alpha)
        {
            //double[] inputs = neuron.Inputs;
            //int neuronNeuronsLength = inputs.Length;
            //int offset = neuron.WeightsOffset;
            //double sum = 0.0;
            //for (int i = 0; i < neuronNeuronsLength; i++)
            //{
            //    sum += inputs[i] * weights[offset + i];
            //}
            //neuron.Outputs[neuron.layerIndex] = 1.0 / (1.0 + Math.Exp(-(alpha * sum)));


            double sum = 0.0;
            for (int i = 0; i < neuron.WeightsLength; i++)
            {
                sum += neuron.Inputs[i] * weights[neuron.WeightsOffset + i];
            }
            neuron.Outputs[neuron.layerIndex] = 1.0 / (1.0 + Math.Exp(-(alpha * sum)));

        }

        private void QueueProcess(GeneticQueue1 queue)
        {
            int queueNeuronsLength = queue.Neurons.Length;
            for (int i = 0; i < queueNeuronsLength; i++)
            {
                GeneiticNeuron1 neuron = queue.Neurons[i];
                NeuronCalculate(neuron, _weights, _alpha);
            }
        }



        //private static void QueueProcess(GeneticQueue queue, double[] weights, double alpha)
        //{
        //    int queueNeuronsLength = queue.Neurons.Length;
        //    for (int i = 0; i < queueNeuronsLength; i++)
        //    {
        //        GeneiticNeuron neuron = queue.Neurons[i];
        //        int neuronNeuronsLength = neuron.Neurons.Length;
        //        GeneiticNeuron[] neurons = neuron.Neurons;
        //        int offset = neuron.WeightsOffset;
        //        double sum = 0.0;
        //        for (int j = 0; j < neuronNeuronsLength; j++)
        //        {
        //            double input = neurons[j].Output;
        //            sum = sum + input * weights[offset + j];
        //        }
        //        neuron.Output = 1.0 / (1.0 + Math.Exp(-(alpha * sum)));
        //    }
        //}



        // Function

        //private static double MathSigmoid(double value)
        //{
        //    //return 1.0 / (1.0 + (double)Math.Exp(-value));
        //    return 1.0 / (1.0 + Math.Exp(-value));
        //}

        public void SetAlpha(double alpha)
        {
            _alpha = alpha;
        }

        //public void SetWeigths(double[] weights, int offset)
        //{
        //    for (int i = 0; i < _queue.Neurons.Length; i++)
        //    {
        //        GeneiticNeuron neuron = _queue.Neurons[i];
        //        //neuron.Weights = weights;
        //        neuron.WeightsOffset = offset;
        //        offset += neuron.WeightsLength;
        //    }
        //}

        public double[] GetWeigths()
        {
            return _weights;
        }

        public void CreateWeigths(int length)
        {
            _weights = new double[length];
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
