using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Intelligence.Random;

namespace MasterDataFlow.Intelligence.Neuron
{
    public class MockNeuron
    {
        public float[] Weights;
        public MockNeuron[] Neurons;
        public float Alpha;
        public float Output;

        public float Error;
        public float[] DeltaWeights;
    }

    public class Network
    {
        public MockNeuron[] Inputs;
        public Layer[] Layers;
        public MockNeuron[] Outputs;
    }

    public class Layer
    {
        public MockNeuron[] Neurons;
    }

    public class Queue 
    {
        public MockNeuron[] Neurons;
    }

    public class Sample
    {
        public float[] Inputs;
        public float[] Outputs;
    }

    public abstract class NeuronNetwork<TValue> : INeuron<TValue>
    {
        private Network _network;
        private Queue _queue;
        private List<Sample> _samples;


        /// <summary>
        /// Инициализация данных для обучения
        /// </summary>
        public void NetworkInitSamples(int maxCount, int inputCount, int outputCount)
        {
            _samples = new List<Sample>(maxCount);
        }

        /// <summary>
        /// Добавление сэмпла
        /// </summary>
        public void NetworkAddSample(float[] inputs, float[] outputs)
        {
            if (_samples.Capacity == _samples.Count)
                return;
            Sample sample = new Sample();
            sample.Inputs = inputs;
            sample.Outputs = outputs;
            _samples.Add(sample);
        }

        public float[] NetworkGetInputSample(int index)
        {
            return _samples[index].Inputs;
        }

        public float[] NetworkGetOutputSample(int index)
        {
            return _samples[index].Outputs;
        }

        public int NetworkGetSamplesCount()
        {
            return _samples.Count;
        }

        public int NetworkGetSamplesMaxCount()
        {
            return _samples.Capacity;
        }


        private MockNeuron CreateFloatNeuron(MockNeuron[] neurons, float[] weigths, float alpha)
        {
            MockNeuron neuron = new MockNeuron();
            neuron.Weights = weigths;
            neuron.DeltaWeights = new float[weigths.Length];
            neuron.Neurons = neurons;
            neuron.Alpha = alpha;
            return neuron;
        }


        private void NetworkCreate(float[] inputs)
        {
            _network = new Network();
            List<MockNeuron> list = new List<MockNeuron>();
            // Создаём входные нейроны, выхода которых
            // является входами для остальных нейронов
            foreach (float input in inputs)
            {
                MockNeuron neuron = new MockNeuron();
                neuron.Output = input;
                list.Add(neuron);
            }
            _network.Inputs = list.ToArray();
        }

        private Queue QueueCreateFromNetwork(Network network)
        {
            List<MockNeuron> list = new List<MockNeuron>();
            for (int i = 0; i < network.Layers.Length; i++)
            {
                Layer layer = network.Layers[i];
                for (int j = 0; j < layer.Neurons.Length; j++)
                {
                    list.Add(layer.Neurons[j]);
                }
            }
            Queue queue = new Queue();
            queue.Neurons = list.ToArray();
            return queue;
        }

        public void NetworkRandomWeights(int seed)
        {
            var random = RandomFactory.Instance.Create();
            for (int i = 0; i < _network.Layers.Length; i++)
            {
                Layer layer = _network.Layers[i];
                for (int j = 0; j < layer.Neurons.Length; j++)
                {
                    MockNeuron neuron = layer.Neurons[j];
                    for (int k = 0; k < neuron.Weights.Length; k++)
                    {
                        neuron.Weights[k] = 1 - (float) random.NextDouble()*2;
                        //neuron.Weights[k] = (float)random.NextDouble();
                    }
                }
            }
        }

        public void NetworkCreate(float alpha, int[] neuronCount)
        {
            _network = new Network();
            if (neuronCount == null || neuronCount.Length < 2)
                throw new Exception("Должно быть как минимум 2 элемента массива для количества нейронов входа и выхода");
            // Создаём входные нейроны, выхода которых
            // является входами для остальных нейронов
            int inputCount = neuronCount[0];
            List<MockNeuron> list = new List<MockNeuron>();
            for (int i = 0; i < inputCount; i++)
            {
                MockNeuron neuron = new MockNeuron();
                list.Add(neuron);
            }
            _network.Inputs = list.ToArray();

            MockNeuron[] previosLayerOutputs = _network.Inputs;

            // Создаём внутренние слои + выходной
            List<Layer> layersList = new List<Layer>();
            for (int i = 1; i < neuronCount.Length; i++)
            {
                int previosLayerNeuronCount = neuronCount[i - 1];
                Layer layer = new Layer();
                List<MockNeuron> layerNeuronsList = new List<MockNeuron>();
                for (int j = 0; j < neuronCount[i]; j++)
                {
                    MockNeuron neuron = new MockNeuron();
                    neuron.Weights = new float[previosLayerNeuronCount];
                    neuron.DeltaWeights = new float[previosLayerNeuronCount];
                    neuron.Neurons = previosLayerOutputs;
                    neuron.Alpha = alpha;
                    layerNeuronsList.Add(neuron);
                }
                layer.Neurons = layerNeuronsList.ToArray();
                previosLayerOutputs = layer.Neurons;
                layersList.Add(layer);
            }
            _network.Layers = layersList.ToArray();
            _network.Outputs = _network.Layers[_network.Layers.Length - 1].Neurons;

            _queue = QueueCreateFromNetwork(_network);
        }

        public int NetworkGetLayersCount()
        {
            if (_network != null)
            {
                if (_network.Layers != null)
                    return _network.Layers.Length;
            }
            return 0;
        }

        public int NetworkGetInputCount()
        {
            if (_network != null)
            {
                if (_network.Inputs != null)
                    return _network.Inputs.Length;
            }
            return 0;
        }

        public int NetworkGetOutputCount()
        {
            if (_network != null)
            {
                if (_network.Outputs != null)
                    return _network.Outputs.Length;
            }
            return 0;
        }


        private float NetworkLearningRunSample(float[] inputs, float[] outputs, float rate, float momentum)
        {
            NetworkCompute(inputs);

            float result = 0;

            // Stage1
            for (int i = 0; i < _network.Outputs.Length; i++)
            {
                MockNeuron neuron = _network.Outputs[i];
                float neuronOutput = neuron.Output;
                // TODO В  разных источниках по разному вычитание
                float error = (outputs[i] - neuronOutput);
                neuron.Error = error*neuron.Alpha*MathSigmoidDerivative(neuronOutput);
                result += error*error;
            }

            // Stage2
            Layer[] layers = _network.Layers;
            for (int i = layers.Length - 2; i >= 0; i--)
            {
                Layer layer = layers[i];
                Layer layerNext = layers[i + 1];
                MockNeuron[] neurons = layer.Neurons;
                MockNeuron[] neuronsNext = layerNext.Neurons;
                for (int j = 0; j < neurons.Length; j++)
                {
                    MockNeuron neuron = neurons[j];
                    float sum = 0.0F;
                    for (int k = 0; k < neuronsNext.Length; k++)
                    {
                        MockNeuron nextNeuron = neuronsNext[k];
                        sum += nextNeuron.Error*nextNeuron.Weights[j];
                    }
                    neuron.Error = sum*neuron.Alpha*MathSigmoidDerivative(neuron.Output);
                }
            }


            // Stage3
            for (int i = 0; i < layers.Length; i++)
            {
                Layer layer = layers[i];
                for (int j = 0; j < layer.Neurons.Length; j++)
                {
                    MockNeuron neuron = layer.Neurons[j];
                    for (int k = 0; k < neuron.Weights.Length; k++)
                    {
//                        float deltaWeight = rate * ((1 - momentum) * neuron.Error * neuron.Neurons[k].Output + momentum * neuron.DeltaWeights[k]);
//                        neuron.DeltaWeights[k] = deltaWeight;
//                        neuron.Weights[k] = neuron.Weights[k] + deltaWeight;
                        float deltaWeight = (1 - momentum)*neuron.Error*neuron.Neurons[k].Output +
                                            momentum*neuron.DeltaWeights[k];
                        neuron.DeltaWeights[k] = deltaWeight;
                        neuron.Weights[k] = neuron.Weights[k] + rate*deltaWeight;
                    }

                }
            }
            return result/2;

        }

        /// <summary>
        /// Запуск обучения
        /// </summary>
        /// <param name="count">Количество циклов</param>
        /// <returns>Среднеквадратичная ошибка обучения</returns>
        public float NetworkLearningRun(int count, float rate, float momentum)
        {
            float result = 0;
            for (int k = 0; k < count; k++)
            {
                result = 0;
                // TODO у нейронов надо обнулить DeltaWeights
                for (int i = 0; i < _samples.Count; i++)
                {
                    result += NetworkLearningRunSample(_samples[i].Inputs, _samples[i].Outputs, rate, momentum);
                }
            }
            return result;
        }

        public TValue[] NetworkCompute(TValue[] inputs)
        {
            if (inputs.Length != _network.Inputs.Length)
                throw new Exception("Несовпадает количество входов сети в количеством входных значений");
            // Заполняет входные данные сети
            for (int i = 0; i < _network.Inputs.Length; i++)
            {
                _network.Inputs[i].Output = inputs[i];
            }
            QueueProcess(_queue);
            float[] result = new float[_network.Outputs.Length];
            for (int i = 0; i < _network.Outputs.Length; i++)
            {
                result[i] = _network.Outputs[i].Output;
            }
            return result;
        }


        private void NeuronCalculate(MockNeuron neuron)
        {
            float sum = 0;
            for (int i = 0; i < neuron.Neurons.Length; i++)
            {
                float input = neuron.Neurons[i].Output;
                sum += input*neuron.Weights[i];
            }
            neuron.Output = MathSigmoid(neuron.Alpha*sum);

        }


        public void NetworkSetNeuronWeights(int layer, int neuron, float[] weigths)
        {
            MockNeuron n = _network.Layers[layer].Neurons[neuron];
            if (weigths.Length != n.Weights.Length)
                throw new Exception("Неверный размер вновь устанавливаемого массива нейронов");
            n.Weights = weigths;
        }

        public float[] NetworkGetNeuronWeights(int layer, int neuron)
        {
            MockNeuron n = _network.Layers[layer].Neurons[neuron];
            return n.Weights;
        }


        private void QueueProcess(Queue queue)
        {
            for (int i = 0; i < queue.Neurons.Length; i++)
            {
                MockNeuron neuron = queue.Neurons[i];
                NeuronCalculate(neuron);
            }
        }

        public float TestFloatNeuron(float[] inputs, float[] weigths, float alpha)
        {
            NetworkCreate(inputs);
            MockNeuron neuron = CreateFloatNeuron(_network.Inputs, weigths, alpha);
            NeuronCalculate(neuron);
            return neuron.Output;
        }

        public void LoadFloatNeuron(float[] inputs, float[] weigths, float alpha, int repeatCount)
        {
            NetworkCreate(inputs);
            List<MockNeuron> neurons = new List<MockNeuron>();
            _queue = new Queue();
            for (int i = 0; i < repeatCount; i++)
            {
                MockNeuron neuron = CreateFloatNeuron(_network.Inputs, weigths, alpha);
                neurons.Add(neuron);
            }
            _queue.Neurons = neurons.ToArray();
        }

        public float CalculateFloatNeuron()
        {
            QueueProcess(_queue);
            return _queue.Neurons[_queue.Neurons.Length - 1].Output;
        }


        // Function

        private static float MathSigmoid(float value)
        {

            //return ((2 / (1 + AsmCall.MathExp(-value))) - 1);
            //return ((2 / (1 + (float)Math.Exp(-value))) - 1);
            return 1/(1 + (float) Math.Exp(-value));
        }

        private static float MathSigmoidDerivative(float value)
        {
            //return (1 - value * value) / 2;
            return value*(1 - value);
        }

        public static float CalculateRawNeuron(float alpha, float[] inputs, float[] weigths)
        {
            float sum = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                sum += inputs[i]*weigths[i];
            }
            return MathSigmoid(alpha*sum);
        }

        public void SetAlpha(float alpha)
        {

            for (int i = 0; i < _queue.Neurons.Length; i++)
            {
                MockNeuron neuron = _queue.Neurons[i];
                neuron.Alpha = alpha;
            }
        }

        public void SetWeigths(float[] weights, int offset)
        {
            throw new NotImplementedException();
        }

    }

}
