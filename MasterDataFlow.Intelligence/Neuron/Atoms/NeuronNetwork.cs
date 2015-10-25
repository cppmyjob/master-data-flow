using System;
using System.Collections.Generic;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Intelligence.Random;

namespace MasterDataFlow.Intelligence.Neuron.Atoms
{
    public class MockNeuron<TValue>
    {
        public TValue[] Weights;
        public MockNeuron<TValue>[] Neurons;
        public TValue Alpha;
        public TValue Output;

        public TValue Error;
        public TValue[] DeltaWeights;
    }

    public class Network<TValue>
    {
        public MockNeuron<TValue>[] Inputs;
        public Layer<TValue>[] Layers;
        public MockNeuron<TValue>[] Outputs;
    }

    public class Layer<TValue>
    {
        public MockNeuron<TValue>[] Neurons;
    }

    public class Queue<TValue> 
    {
        public MockNeuron<TValue>[] Neurons;
    }

    public class Sample<TValue>
    {
        public TValue[] Inputs;
        public TValue[] Outputs;
    }

    public abstract class NeuronNetwork<TValue> : INeuron<TValue>
    {
        //private Func<double, double, double> Substract = (value1, value2) => value1 - value2;
        //private Func<float, float, float> Substract = (value1, value2) => value1 - value2;

        private Network<TValue> _network;
        private Queue<TValue> _queue;
        private List<Sample<TValue>> _samples;


        /// <summary>
        /// Инициализация данных для обучения
        /// </summary>
        public void NetworkInitSamples(int maxCount, int inputCount, int outputCount)
        {
            _samples = new List<Sample<TValue>>(maxCount);
        }

        /// <summary>
        /// Добавление сэмпла
        /// </summary>
        public void NetworkAddSample(TValue[] inputs, TValue[] outputs)
        {
            if (_samples.Capacity == _samples.Count)
                return;
            var sample = new Sample<TValue>();
            sample.Inputs = inputs;
            sample.Outputs = outputs;
            _samples.Add(sample);
        }

        public TValue[] NetworkGetInputSample(int index)
        {
            return _samples[index].Inputs;
        }

        public TValue[] NetworkGetOutputSample(int index)
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


        private MockNeuron<TValue> CreateFloatNeuron(MockNeuron<TValue>[] neurons, TValue[] weigths, TValue alpha)
        {
            var neuron = new MockNeuron<TValue>();
            neuron.Weights = weigths;
            neuron.DeltaWeights = new TValue[weigths.Length];
            neuron.Neurons = neurons;
            neuron.Alpha = alpha;
            return neuron;
        }


        private void NetworkCreate(TValue[] inputs)
        {
            _network = new Network<TValue>();
            var list = new List<MockNeuron<TValue>>();
            // Создаём входные нейроны, выхода которых
            // является входами для остальных нейронов
            foreach (var input in inputs)
            {
                var neuron = new MockNeuron<TValue>();
                neuron.Output = input;
                list.Add(neuron);
            }
            _network.Inputs = list.ToArray();
        }

        private Queue<TValue> QueueCreateFromNetwork(Network<TValue> network)
        {
            var list = new List<MockNeuron<TValue>>();
            for (int i = 0; i < network.Layers.Length; i++)
            {
                var layer = network.Layers[i];
                for (int j = 0; j < layer.Neurons.Length; j++)
                {
                    list.Add(layer.Neurons[j]);
                }
            }
            var queue = new Queue<TValue>();
            queue.Neurons = list.ToArray();
            return queue;
        }

        public void NetworkRandomWeights(int seed)
        {
            var random = RandomFactory.Instance.Create();
            for (var i = 0; i < _network.Layers.Length; i++)
            {
                var layer = _network.Layers[i];
                for (var j = 0; j < layer.Neurons.Length; j++)
                {
                    var neuron = layer.Neurons[j];
                    for (int k = 0; k < neuron.Weights.Length; k++)
                    {
                        //neuron.Weights[k] = 1 - random.NextDouble() * 2;

                        //neuron.Weights[k] = (float)random.NextDouble();
                    }
                }
            }
        }

        public void NetworkCreate(TValue alpha, int[] neuronCount)
        {
            _network = new Network<TValue>();
            if (neuronCount == null || neuronCount.Length < 2)
                throw new Exception("Должно быть как минимум 2 элемента массива для количества нейронов входа и выхода");
            // Создаём входные нейроны, выхода которых
            // является входами для остальных нейронов
            int inputCount = neuronCount[0];
            var list = new List<MockNeuron<TValue>>();
            for (int i = 0; i < inputCount; i++)
            {
                var neuron = new MockNeuron<TValue>();
                list.Add(neuron);
            }
            _network.Inputs = list.ToArray();

            var previosLayerOutputs = _network.Inputs;

            // Создаём внутренние слои + выходной
            var layersList = new List<Layer<TValue>>();
            for (int i = 1; i < neuronCount.Length; i++)
            {
                int previosLayerNeuronCount = neuronCount[i - 1];
                var layer = new Layer<TValue>();
                var layerNeuronsList = new List<MockNeuron<TValue>>();
                for (int j = 0; j < neuronCount[i]; j++)
                {
                    var neuron = new MockNeuron<TValue>();
                    neuron.Weights = new TValue[previosLayerNeuronCount];
                    neuron.DeltaWeights = new TValue[previosLayerNeuronCount];
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

        protected abstract TValue Substract(TValue value1, TValue value2);
        protected abstract TValue Substract(int value1, TValue value2);
        protected abstract TValue Multiple(TValue value1, TValue value2, TValue value3);
        protected abstract TValue Multiple(TValue value1, TValue value2);
        protected abstract TValue Add(TValue value1, TValue value2);
        protected abstract TValue Add(int value1, TValue value2);
        protected abstract TValue Divide(TValue value1, int value2);
        protected abstract double Negative(TValue value);
        protected abstract TValue ToValue(double value);

        private TValue NetworkLearningRunSample(TValue[] inputs, TValue[] outputs, TValue rate, TValue momentum)
        {
            NetworkCompute(inputs);

            TValue result = default(TValue);

            // Stage1
            for (int i = 0; i < _network.Outputs.Length; i++)
            {
                var neuron = _network.Outputs[i];
                TValue neuronOutput = neuron.Output;
                // TODO В  разных источниках по разному вычитание
                //var error = (outputs[i] - neuronOutput);
                var error = Substract(outputs[i], neuronOutput);
                //neuron.Error = error*neuron.Alpha*MathSigmoidDerivative(neuronOutput);
                neuron.Error = Multiple(error, neuron.Alpha, MathSigmoidDerivative(neuronOutput));
                //result += error * error;
                result = Add(result, Multiple(error, error));
            }

            // Stage2
            var layers = _network.Layers;
            for (int i = layers.Length - 2; i >= 0; i--)
            {
                var layer = layers[i];
                var layerNext = layers[i + 1];
                var neurons = layer.Neurons;
                var neuronsNext = layerNext.Neurons;
                for (int j = 0; j < neurons.Length; j++)
                {
                    var neuron = neurons[j];
                    TValue sum = default(TValue);
                    for (int k = 0; k < neuronsNext.Length; k++)
                    {
                        var nextNeuron = neuronsNext[k];
                        //sum += nextNeuron.Error*nextNeuron.Weights[j];
                        sum = Add(sum, Multiple(nextNeuron.Error, nextNeuron.Weights[j]));
                    }
                    //neuron.Error = sum*neuron.Alpha*MathSigmoidDerivative(neuron.Output);
                    neuron.Error = Multiple(sum, neuron.Alpha, MathSigmoidDerivative(neuron.Output));
                }
            }


            // Stage3
            for (int i = 0; i < layers.Length; i++)
            {
                var layer = layers[i];
                for (int j = 0; j < layer.Neurons.Length; j++)
                {
                    var neuron = layer.Neurons[j];
                    for (int k = 0; k < neuron.Weights.Length; k++)
                    {
//                        float deltaWeight = rate * ((1 - momentum) * neuron.Error * neuron.Neurons[k].Output + momentum * neuron.DeltaWeights[k]);
//                        neuron.DeltaWeights[k] = deltaWeight;
//                        neuron.Weights[k] = neuron.Weights[k] + deltaWeight;
                        TValue deltaWeight = Add(Multiple(Substract(1, momentum), neuron.Error, neuron.Neurons[k].Output), 
                                            Multiple(momentum, neuron.DeltaWeights[k]));
                        neuron.DeltaWeights[k] = deltaWeight;
                        neuron.Weights[k] = Add(neuron.Weights[k], Multiple(rate, deltaWeight));
                    }

                }
            }
            return Divide(result, 2);

        }

        /// <summary>
        /// Запуск обучения
        /// </summary>
        /// <param name="count">Количество циклов</param>
        /// <returns>Среднеквадратичная ошибка обучения</returns>
        public TValue NetworkLearningRun(int count, TValue rate, TValue momentum)
        {
            TValue result = default(TValue);
            for (int k = 0; k < count; k++)
            {
                // TODO ????????????
                result = default(TValue);
                // TODO у нейронов надо обнулить DeltaWeights
                for (int i = 0; i < _samples.Count; i++)
                {
                    //result += NetworkLearningRunSample(_samples[i].Inputs, _samples[i].Outputs, rate, momentum);
                    result = Add(result, NetworkLearningRunSample(_samples[i].Inputs, _samples[i].Outputs, rate, momentum));
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
            var result = new TValue[_network.Outputs.Length];
            for (int i = 0; i < _network.Outputs.Length; i++)
            {
                result[i] = _network.Outputs[i].Output;
            }
            return result;
        }


        private void NeuronCalculate(MockNeuron<TValue> neuron)
        {
            TValue sum = default(TValue);
            for (int i = 0; i < neuron.Neurons.Length; i++)
            {
                TValue input = neuron.Neurons[i].Output;
                //sum += input*neuron.Weights[i];
                sum = Add(sum, Multiple(input, neuron.Weights[i]));
            }
            //neuron.Output = MathSigmoid(neuron.Alpha*sum);
            neuron.Output = MathSigmoid(Multiple(neuron.Alpha, sum));

        }


        public void NetworkSetNeuronWeights(int layer, int neuron, TValue[] weigths)
        {
            var n = _network.Layers[layer].Neurons[neuron];
            if (weigths.Length != n.Weights.Length)
                throw new Exception("Неверный размер вновь устанавливаемого массива нейронов");
            n.Weights = weigths;
        }

        public TValue[] NetworkGetNeuronWeights(int layer, int neuron)
        {
            var n = _network.Layers[layer].Neurons[neuron];
            return n.Weights;
        }


        private void QueueProcess(Queue<TValue> queue)
        {
            for (int i = 0; i < queue.Neurons.Length; i++)
            {
                var neuron = queue.Neurons[i];
                NeuronCalculate(neuron);
            }
        }

        public TValue TestFloatNeuron(TValue[] inputs, TValue[] weigths, TValue alpha)
        {
            NetworkCreate(inputs);
            var neuron = CreateFloatNeuron(_network.Inputs, weigths, alpha);
            NeuronCalculate(neuron);
            return neuron.Output;
        }

        public void LoadFloatNeuron(TValue[] inputs, TValue[] weigths, TValue alpha, int repeatCount)
        {
            NetworkCreate(inputs);
            var neurons = new List<MockNeuron<TValue>>();
            _queue = new Queue<TValue>();
            for (int i = 0; i < repeatCount; i++)
            {
                var neuron = CreateFloatNeuron(_network.Inputs, weigths, alpha);
                neurons.Add(neuron);
            }
            _queue.Neurons = neurons.ToArray();
        }

        public TValue CalculateFloatNeuron()
        {
            QueueProcess(_queue);
            return _queue.Neurons[_queue.Neurons.Length - 1].Output;
        }


        // Function

        private TValue MathSigmoid(TValue value)
        {

            //return ((2 / (1 + AsmCall.MathExp(-value))) - 1);
            //return ((2 / (1 + (float)Math.Exp(-value))) - 1);
            //return 1 / (1 + Math.Exp(-value));
            return ToValue(1 / (1 + Math.Exp(Negative(value))));
        }

        private TValue MathSigmoidDerivative(TValue value)
        {
            //return (1 - value * value) / 2;
            //return value*(1 - value);
            return Multiple(value, Substract(1, value));
        }

        public TValue CalculateRawNeuron(TValue alpha, TValue[] inputs, TValue[] weigths)
        {
            TValue sum = default(TValue);
            for (int i = 0; i < inputs.Length; i++)
            {
                sum = Add(sum, Multiple(inputs[i], weigths[i]));
            }
            //return MathSigmoid(alpha*sum);
            //return MathSigmoid(alpha * sum);
            return MathSigmoid(Multiple(alpha, sum));
        }

        public void SetAlpha(TValue alpha)
        {

            for (int i = 0; i < _queue.Neurons.Length; i++)
            {
                var neuron = _queue.Neurons[i];
                neuron.Alpha = alpha;
            }
        }

        public void SetWeigths(float[] weights, int offset)
        {
            throw new NotImplementedException();
        }

    }

}
