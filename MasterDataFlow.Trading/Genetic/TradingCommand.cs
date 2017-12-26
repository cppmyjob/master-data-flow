using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Intelligence.Neuron;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Genetic
{

    [Serializable]
    public class LearningDataIndicator
    {
        public string Name { get; set; }
        public float[] Values { get; set; }
        public DateTime[] Times { get; set; }
    }

    [Serializable]
    public class ZigZagValue
    {
        public int Value { get; set; }
        public DateTime Time { get; set; }
    }

    [Serializable]
    public class LearningData
    {
        public Bar[] Prices { get; set; }
        public LearningDataIndicator[] Indicators { get; set; }
        public ZigZagValue[] ZigZags { get; set; }
    }

    [Serializable]
    public class NeuronNetworkLevel
    {
        public int Length { get; set; }
    }


    [Serializable]
    public class NeuronNetwork
    {
        public NeuronNetworkLevel[] Layers { get; set; }

        public NeuronNetwork()
        {
            Layers = NeuronsConfig.Select(t => new NeuronNetworkLevel { Length = t })
                .ToArray();
        }

        public NeuronNetwork(int[] config)
        {
            Layers = config.Select(t => new NeuronNetworkLevel {Length = t})
                .ToArray();
        }

        public int[] GetNeuronsConfig()
        {
            return Layers.Select(t => t.Length).ToArray();
        }

        public int GetWeightsCount()
        {
            int[] neurons = GetNeuronsConfig();
            int result = 0;
            for (int i = 1; i < neurons.Length; i++)
            {
                result += neurons[i - 1] * neurons[i];
            }
            return result;
        }

        public const int HISTORY_WINDOW_LENGTH = 48;

        private static int[] NeuronsConfig = new int[] {
            HISTORY_WINDOW_LENGTH * TradingItemInitData.INDICATOR_NUMBER,
            1 * (HISTORY_WINDOW_LENGTH * TradingItemInitData.INDICATOR_NUMBER),
            2 * (HISTORY_WINDOW_LENGTH * TradingItemInitData.INDICATOR_NUMBER),
            1 * (HISTORY_WINDOW_LENGTH * TradingItemInitData.INDICATOR_NUMBER),
            2,
        };

        public void Read(XElement root)
        {
            var networkElement = root.Element("neuronNetwork");

            var networkLayers = new List<NeuronNetworkLevel>();

            var levels = networkElement.Element("levels");
            foreach (var eValue in levels.Elements("level"))
            {
                networkLayers.Add(new NeuronNetworkLevel { Length = Convert.ToInt32(eValue.Value) });
            }

            Layers = networkLayers.ToArray();
        }

        public void Write(XElement root)
        {
            var neuronNetwork = new XElement("neuronNetwork");
            root.Add(neuronNetwork);

            var levels = new XElement("levels");
            neuronNetwork.Add(levels);

            var neuronConfig = GetNeuronsConfig();
            for (int i = 0; i < neuronConfig.Length; i++)
            {
                levels.Add(new XElement("level", neuronConfig[i].ToString()));
            }
        }

    }


    [Serializable]
    public class TradingItemInitData : GeneticItemInitData
    {
        public const int INDICATOR_NUMBER = 5;

        private NeuronNetwork _neuronNetwork;
        private int _historyWidowLength;

        private double _indicatorMutation = 0.999;
        private double _valuesMutation = 0.999;
        private double _otherValuesMutation = 0.999;
        private int _indicatorNumber = INDICATOR_NUMBER;

        public const int MAX_STOPLOSS = 100;

        public const int OFFSET_INDICATOR = 0;

        public int OFFSET_ALPHA
        {
            get { return OFFSET_INDICATOR + _indicatorNumber; }
        }

        public int OFFSET_STOPLOSS
        {
            get { return OFFSET_INDICATOR + _indicatorNumber + 1; }
        }

        public int OFFSET_VALUES
        {
            get { return OFFSET_INDICATOR + _indicatorNumber + 2; }
        }

        public TradingItemInitData() : this(new NeuronNetwork())
        {
        }

        protected TradingItemInitData(NeuronNetwork neuronNetwork) : base(neuronNetwork.GetWeightsCount() + INDICATOR_NUMBER + 2, false)
        {
            _neuronNetwork = neuronNetwork;
            _historyWidowLength = NeuronNetwork.HISTORY_WINDOW_LENGTH;
        }

        public int HistoryWidowLength
        {
            get { return _historyWidowLength; }
        }

        public NeuronNetwork NeuronNetwork
        {
            get { return _neuronNetwork; }
        }

        public double IndicatorMutation
        {
            get { return _indicatorMutation; }
        }

        public double ValuesMutation
        {
            get { return _valuesMutation; }
        }

        public double OtherValuesMutation
        {
            get { return _otherValuesMutation; }
        }

        public int IndicatorNumber
        {
            get { return _indicatorNumber; }
        }

        public void Read(XElement root)
        {
            var eItemInitData = root.Element("itemInitData");

            _neuronNetwork = new NeuronNetwork();
            _neuronNetwork.Read(eItemInitData);

            _indicatorNumber = int.Parse(eItemInitData.Element("indicatorNumber").Value);

            var eHistoryWidowLength = eItemInitData.Element("historyWidowLength");
            _historyWidowLength = int.Parse(eHistoryWidowLength.Value);

            _indicatorMutation = double.Parse(eItemInitData.Element("indicatorMutation").Value);
            _otherValuesMutation = double.Parse(eItemInitData.Element("otherValuesMutation").Value);
            _valuesMutation = double.Parse(eItemInitData.Element("valuesMutation").Value);

            _count = _neuronNetwork.GetWeightsCount() + OFFSET_VALUES;
        }

        public void Write(XElement root)
        {
            var itemInitDataElement = new XElement("itemInitData");
            root.Add(itemInitDataElement);
            _neuronNetwork.Write(itemInitDataElement);

            itemInitDataElement.Add(new XElement("indicatorNumber", _indicatorNumber.ToString(CultureInfo.InvariantCulture)));

            itemInitDataElement.Add(new XElement("historyWidowLength", _historyWidowLength.ToString(CultureInfo.InvariantCulture)));

            itemInitDataElement.Add(new XElement("indicatorMutation", _indicatorMutation.ToString(CultureInfo.InvariantCulture)));
            itemInitDataElement.Add(new XElement("otherValuesMutation", _otherValuesMutation.ToString(CultureInfo.InvariantCulture)));
            itemInitDataElement.Add(new XElement("valuesMutation", _valuesMutation.ToString(CultureInfo.InvariantCulture)));
        }

    }


    [Serializable]
    public class TradingDataObject : GeneticFloatDataObject<TradingItemInitData, TradingItem>
    {
        public LearningData TrainingData { get; }
        public LearningData ValidationData { get; }

        public TradingDataObject(TradingItemInitData itemInitData, LearningData trainingData, LearningData validationData,
            int itemsCount, int surviveCount)
        {
            TrainingData = trainingData;
            ValidationData = validationData;
            ItemInitData = itemInitData;
            CommandInitData = new GeneticCommandInitData(itemsCount, surviveCount, 3000000);
        }
    }

    [Serializable]
    public class TradingItem : GeneticFloatItem<TradingItemInitData>
    {
        public TradingItem(TradingItemInitData initData) : base(initData)
        {
        }

        public override float CreateValue(IRandom random)
        {
            return (float)random.NextDouble();
        }

        public float GetIndicatorIndex(int index)
        {
            return Values[index];
        }

        public float Alpha
        {
            get
            {
                return Values[InitData.OFFSET_ALPHA] * 5;
            }
        }

        public float StopLoss
        {
            get { return Values[InitData.OFFSET_STOPLOSS] * TradingItemInitData.MAX_STOPLOSS; }
        }

        public TesterResult ValidationTesterResult { get; set; }

        public TesterResult TrainingTesterResult { get; set; }
    }


    public class TradingCommand : GeneticFloatCommand<TradingDataObject, TradingItemInitData, TradingItem>
    {
        private Dictionary<int, GeneticNeuronDLL1> _dlls = new Dictionary<int, GeneticNeuronDLL1>();

        protected override TradingItem CreateItem(TradingItemInitData initData)
        {
            return new TradingItem(initData);
        }

        public override double CalculateFitness(TradingItem item, int processor)
        {
            bool[] oldValues = new bool[DataObject.TrainingData.Indicators.Length];
            for (int i = 0; i < item.InitData.IndicatorNumber; i++)
            {
                var index = (int)item.Values[i];
                if (oldValues[index])
                    return Double.MinValue;
                oldValues[index] = true;
            }


            //if (item.StopLoss < 30 || item.StopLoss > 100000)
            //{
            //    return Double.MinValue;
            //}

            var dll = GetNeuronDll(item);
            double validationZigZagCount;
            var validationResult = GetProfit(dll, item, DataObject.ValidationData, out validationZigZagCount);
            if (validationZigZagCount < 0)
                return Double.MinValue;
            item.ValidationTesterResult = validationResult;

            if (FilterBadResult(validationResult))
                return Double.MinValue;

            double trainingZigZagCount;
            var trainingResult = GetProfit(dll, item, DataObject.TrainingData, out trainingZigZagCount);
            if (trainingZigZagCount < 0)
                return Double.MinValue;
            item.TrainingTesterResult = trainingResult;


            if (validationResult.MinusCount > 0 && trainingResult.MinusCount > 0)
            {
                if (((float)validationResult.PlusCount / validationResult.MinusCount) <
                    ((float)trainingResult.PlusCount / trainingResult.MinusCount))
                {
                    return Double.MinValue;
                }
            }

            //if (trainingResult.MinusEquityCount > trainingResult.PlusEquityCount)
            //{
            //    return Double.MinValue;
            //}

            if (trainingResult.Profit <= 0)
            {
                return Double.MinValue;
            }


            //if (validationResult.MinusCount == 0 || trainingResult.MinusCount == 0)
            //    return Double.MinValue;


            var m = 1m;

            //var m = (validationResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit) +
            //         trainingResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit)) /
            //        (validationResult.Orders.Count + trainingResult.Orders.Count);

            var pmRatio = ((double) (trainingResult.PlusCount + validationResult.PlusCount) /
                           ((trainingResult.MinusCount + validationResult.MinusCount) > 0 ? (trainingResult.MinusCount + validationResult.MinusCount) : 1) );

            var ecRation = 1;

            //var ecRation = ((double)(trainingResult.PlusEquityCount + validationResult.PlusEquityCount) /
            //            ((trainingResult.MinusEquityCount + validationResult.MinusEquityCount) > 0 ? (trainingResult.MinusEquityCount + validationResult.MinusEquityCount) : 1));

            return //(double)(validationResult.Profit + trainingResult.Profit)
                   //* (double) (TradingItemInitData.MAX_STOPLOSS - item.StopLoss)
                   //* (double)(trainingResult.OrderCount + validationResult.OrderCount)
                   //* (double)(trainingResult.PlusCount - trainingResult.MinusCount + validationResult.PlusCount - validationResult.MinusCount)
                   //* (double)pmRatio
                   //* (double)ecRation
                   //* (double)m
                   //* 
                   (double)(validationZigZagCount + trainingZigZagCount);
        }

        private bool FilterBadResult(TesterResult testerResult)
        {
            if (testerResult.Profit <= 0)
            {
                    return true;
            }
            if (testerResult.PlusCount < testerResult.MinusCount)
            {
                    return true;
            }
            if ((float) testerResult.MinusCount / (testerResult.PlusCount + testerResult.MinusCount) > 0.5)
            {
                    return true;
            }

            if (testerResult.BuyCount == 0 || testerResult.SellCount == 0)
            {
                    return true;
            }

            //if (testerResult.MinusEquityCount > testerResult.PlusEquityCount)
            //{
            //    return true;
            //}

            //if (testerResult.BuyCount > testerResult.SellCount)
            //{
            //    if (testerResult.BuyCount / (float)testerResult.SellCount > 2)
            //    {
            //        return true;
            //    }
            //}
            //else
            //{
            //    if (testerResult.SellCount / (float)testerResult.BuyCount > 2)
            //    {
            //        return true;
            //    }
            //}
            return false;
        }

        private TesterResult GetProfit(GeneticNeuronDLL1 dll, TradingItem item, LearningData learningData, out double zigZagCount)
        {
            var tester = new DirectionTester(dll, item, learningData);
            TesterResult result = tester.Run();
            zigZagCount = tester.ZigZagCount;
            return result;
        }

        private GeneticNeuronDLL1 GetNeuronDll(TradingItem item)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            GeneticNeuronDLL1 dll;
            lock (_dlls) // TODO improve
            {
                if (!_dlls.TryGetValue(threadId, out dll))
                {
                    dll = CreateNeuronDll(DataObject);
                    _dlls.Add(threadId, dll);
                }
            }
            dll.SetAlpha(item.Alpha);
            SetWeigths(dll, item);
            return dll;
        }

        public static GeneticNeuronDLL1 CreateNeuronDll(TradingDataObject dataObject, TradingItem item)
        {
            var dll = CreateNeuronDll(dataObject);
            dll.SetAlpha(item.Alpha);
            SetWeigths(dll, item);
            return dll;
        }

        private static GeneticNeuronDLL1 CreateNeuronDll(TradingDataObject dataObject)
        {
            GeneticNeuronDLL1 dll = new GeneticNeuronDLL1();
            float alpha = 1.5F;
            dll.NetworkCreate(alpha, dataObject.ItemInitData.NeuronNetwork.GetNeuronsConfig());
            dll.CreateWeigths(dataObject.ItemInitData.NeuronNetwork.GetWeightsCount());
            return dll;
        }

        private static void SetWeigths(GeneticNeuronDLL1 dll, TradingItem item)
        {
            var weights = dll.GetWeigths();
            var values = item.Values;

            //Parallel.For(0, weights.Length, i => weights[i] = values[i + TradingItem.OFFSET_VALUES] * 100.0F - 50.0F);

            var offset = item.InitData.OFFSET_VALUES;
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = values[offset] * 2000.0F - 1000.0F;
                //weights[i] = values[offset] * 2.0F - 1.0F;
                ++offset;
            }
        }

        protected override void FillValues(TradingItem item)
        {
            var values = item.Values;
            var allIndexes = GetUniqueIndicatorIndexes();
            for (int i = 0; i < item.InitData.IndicatorNumber; i++)
            {
                values[i] = allIndexes[i];
            }

            for (int j = item.InitData.OFFSET_ALPHA; j < DataObject.ItemInitData.Count; j++)
            {
                var valueValue = item.CreateValue(Random);
                values[j] = valueValue;
            }
        }

        protected override void Mutation(TradingItem item)
        {
            if (Random.NextDouble() > item.InitData.IndicatorMutation)
            {
                var allIndexes = GetUniqueIndicatorIndexes();
                for (int i = 0; i < item.InitData.IndicatorNumber; i++)
                {
                    item.Values[i] = allIndexes[i];
                }
            }

            for (int i = item.InitData.OFFSET_ALPHA; i < item.InitData.OFFSET_VALUES; i++)
            {
                if (Random.NextDouble() > item.InitData.OtherValuesMutation)
                {
                    var valueValue = item.CreateValue(Random);
                    item.Values[i] = valueValue;
                }
            }

            for (int i = item.InitData.OFFSET_VALUES; i < item.Values.Length; i++)
            {
                if (Random.NextDouble() > item.InitData.ValuesMutation)
                {
                    var valueValue = item.CreateValue(Random);
                    item.Values[i] = valueValue;
                }
            }
        }

        protected override TradingItem CreateChild(TradingItem firstParent, TradingItem secondParent)
        {
            var child = InternalCreateItem();
            float[] firstValues = firstParent.Values;
            float[] secondValues = secondParent.Values;
            float[] childValues = child.Values;

            for (int i = 0; i < child.InitData.OFFSET_VALUES; i++)
            {
                if (i % 2 == 0)
                {
                    childValues[i] = firstValues[i];
                }
                else
                {
                    childValues[i] = secondValues[i];
                }
            }

            int[] neurons = DataObject.ItemInitData.NeuronNetwork.GetNeuronsConfig();
            var offset = child.InitData.OFFSET_VALUES;
            for (int i = 1; i < neurons.Length; i++)
            {
                var layerWeights = neurons[i - 1] * neurons[i];
                if (i % 2 == 0)
                {
                    Array.Copy(firstValues, offset, childValues, offset, layerWeights);
                }
                else
                {
                    Array.Copy(secondValues, offset, childValues, offset, layerWeights);
                }
                offset += layerWeights;
            }

            return child;
        }


        private int[] GetUniqueIndicatorIndexes()
        {
            var result = new int[DataObject.TrainingData.Indicators.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = i;
            }

            for (int i = result.Length; i > 0; i--)
            {
                int j = Random.Next(i);
                int k = (int)result[j];
                result[j] = result[i - 1];
                result[i - 1] = k;
            }
            return result;
        }
    }
}
