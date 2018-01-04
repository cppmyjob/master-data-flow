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
using MasterDataFlow.Trading.Configs;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Genetic
{

    [Serializable]
    public class LearningDataIndicator :  IComparable
    {
        public class IndicaorSearch
        {
            private string _s;

            public IndicaorSearch(string s)
            {
                _s = s;
            }

            public bool Match(LearningDataIndicator e)
            {
                return e.Name == _s;
            }
        }

        public string Name { get; set; }
        public float[] Values { get; set; }
        public DateTime[] Times { get; set; }

        public void Normalize()
        {
            var min = Values.Min();
            var max = Values.Max();
            var diff = max - min;
            var offset = diff * 20 / 100;
            min = min - offset;
            max = max + offset;

            diff = max - min;

            Values = Values.Select(t => (t - min) / diff ).ToArray();

        }

        public int CompareTo(object o)
        {
            if (!(o is LearningDataIndicator indicator))
                throw new ArgumentException("o is not an LearningDataIndicator object.");

            return String.Compare(Name, indicator.Name, StringComparison.Ordinal);
        }
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
        //public LearningDataIndicator Values { get; set; }
        public ZigZagValue[] ZigZags { get; set; }
    }

    [Serializable]
    public class NeuronNetworkLevel
    {
        public int Length { get; set; }
    }

    [Serializable]
    public class Indicators
    {
        public const int INDICATOR_NUMBER = 5;

        public bool IsNormalizeValues { get; private set; } = true;
        public int IndicatorNumber { get; private set; } = INDICATOR_NUMBER;
        public string PredefinedNames { get; private set; } = String.Empty;

        public void Read(XElement root)
        {
            var eIndicators = root.Element("indicators");
            if (eIndicators == null)
                return;
            IndicatorNumber = Convert.ToInt32(eIndicators.Element("indicatorNumber").Value);
            IsNormalizeValues = Convert.ToBoolean(eIndicators.Element("isNormalizeValues").Value);

            var ePredefinedNames = eIndicators.Element("predefinedNames");
            if (ePredefinedNames != null)
            {
                PredefinedNames = ePredefinedNames.Value;
            }
        }

        public void Write(XElement root)
        {
            var eIndicators = new XElement("indicators");
            root.Add(eIndicators);

            eIndicators.Add(new XElement("indicatorNumber", IndicatorNumber.ToString(CultureInfo.InvariantCulture)));
            eIndicators.Add(new XElement("isNormalizeValues", IsNormalizeValues.ToString(CultureInfo.InvariantCulture)));
            eIndicators.Add(new XElement("predefinedNames", PredefinedNames));
        }

    }

    [Serializable]
    public class InputData
    {
        public Indicators Indicators { get; } = new Indicators();

        public void Read(XElement root)
        {
            var eInputData = root.Element("inputData");
            if (eInputData == null)
                return;
            Indicators.Read(eInputData);
        }

        public void Write(XElement root)
        {
            var eInputData = new XElement("inputData");
            root.Add(eInputData);
            Indicators.Write(eInputData);
        }
    }

    [Serializable]
    public class ValidationOptimizer
    {
        public ValidationOptimizer()
        {
            var configSection = ItemInitDataConfigSection.GetConfig();
            if (configSection != null)
            {
                IsFilterBadResult = configSection.Optimizer.Training.IsFilterBadResult;
            }
        }


        public bool IsFilterBadResult { get; private set; } = true;

        public void Read(XElement root)
        {
            var eValidation = root.Element("validation");
            if (eValidation == null)
                return;

            IsFilterBadResult = Convert.ToBoolean(eValidation.Element("isFilterBadResult").Value);

        }

        public void Write(XElement root)
        {
            var eValidation = new XElement("validation");
            root.Add(eValidation);

            eValidation.Add(new XElement("isFilterBadResult", IsFilterBadResult.ToString(CultureInfo.InvariantCulture)));
        }

    }

    [Serializable]
    public class TrainingOptimizer
    {
        public TrainingOptimizer()
        {
            var configSection = ItemInitDataConfigSection.GetConfig();
            if (configSection != null)
            {
                IsFilterBadResult = configSection.Optimizer.Training.IsFilterBadResult;
                IsFilterBadResultBuySell = configSection.Optimizer.Training.IsFilterBadResultBuySell;
            }
        }


        public bool IsFilterBadResult { get; private set; } = true;
        public bool IsFilterBadResultBuySell { get; set; } = false;

        public void Read(XElement root)
        {
            var eTraining = root.Element("training");
            if (eTraining == null)
                return;

            IsFilterBadResult = Convert.ToBoolean(eTraining.Element("isFilterBadResult").Value);
            IsFilterBadResultBuySell = Convert.ToBoolean(eTraining.Element("isFilterBadResultBuySell").Value);

        }

        public void Write(XElement root)
        {
            var eTraining = new XElement("training");
            root.Add(eTraining);

            eTraining.Add(new XElement("isFilterBadResult", IsFilterBadResult.ToString(CultureInfo.InvariantCulture)));
            eTraining.Add(new XElement("isFilterBadResultBuySell", IsFilterBadResultBuySell.ToString(CultureInfo.InvariantCulture)));
        }
    }

    [Serializable]
    public class FitnessOptimizer
    {
        public FitnessOptimizer()
        {
            var configSection = ItemInitDataConfigSection.GetConfig();
            if (configSection != null)
            {
                IsExpectedValue = configSection.Optimizer.Fitness.IsExpectedValue;
                IsPlusMinusOrdersRatio = configSection.Optimizer.Fitness.IsPlusMinusOrdersRatio;
                IsPlusMinusEquityRatio = configSection.Optimizer.Fitness.IsPlusMinusEquityRatio;
                IsProfit = configSection.Optimizer.Fitness.IsProfit;
                IsZigZag = configSection.Optimizer.Fitness.IsZigZag;
            }
        }


        public bool IsExpectedValue { get; private set; } = false;
        public bool IsPlusMinusOrdersRatio { get; private set; } = false;
        public bool IsPlusMinusEquityRatio { get; private set; } = false;
        public bool IsProfit { get; private set; } = false;
        public bool IsZigZag { get; private set; } = true;


        public void Read(XElement root)
        {
            var eFitness = root.Element("fitness");
            if (eFitness == null)
                return;

            IsExpectedValue = Convert.ToBoolean(eFitness.Element("isExpectedValue").Value);
            IsPlusMinusOrdersRatio = Convert.ToBoolean(eFitness.Element("isPlusMinusOrdersRatio").Value);
            IsPlusMinusEquityRatio = Convert.ToBoolean(eFitness.Element("isPlusMinusEquityRatio").Value);
            IsProfit = Convert.ToBoolean(eFitness.Element("isProfit").Value);
            IsZigZag = Convert.ToBoolean(eFitness.Element("isZigZag").Value);
        }

        public void Write(XElement root)
        {
            var eFitness = new XElement("fitness");
            root.Add(eFitness);

            eFitness.Add(new XElement("isExpectedValue", IsExpectedValue.ToString(CultureInfo.InvariantCulture)));
            eFitness.Add(new XElement("isPlusMinusOrdersRatio", IsPlusMinusOrdersRatio.ToString(CultureInfo.InvariantCulture)));
            eFitness.Add(new XElement("isPlusMinusEquityRatio", IsPlusMinusEquityRatio.ToString(CultureInfo.InvariantCulture)));
            eFitness.Add(new XElement("isProfit", IsProfit.ToString(CultureInfo.InvariantCulture)));
            eFitness.Add(new XElement("isZigZag", IsZigZag.ToString(CultureInfo.InvariantCulture)));
        }
    }

    [Serializable]
    public class Optimizer {

        public Optimizer()
        {
            var configSection = ItemInitDataConfigSection.GetConfig();
            if (configSection != null)
            {
                IsValidationPlusMinusRatioLessTraining = configSection.Optimizer.IsValidationPlusMinusRatioLessTraining;
            }
        }

        public ValidationOptimizer Validation { get; } = new ValidationOptimizer();
        public TrainingOptimizer Training { get; } = new TrainingOptimizer();
        public FitnessOptimizer Fitness { get; } = new FitnessOptimizer();

        public bool IsValidationPlusMinusRatioLessTraining { get; set; } = true;


        public void Read(XElement root)
        {
            var eOptimizer = root.Element("optimizer");
            if (eOptimizer == null)
                return;

            Validation.Read(eOptimizer);
            Training.Read(eOptimizer);
            Fitness.Read(eOptimizer);
        }

        public void Write(XElement root)
        {
            var eOptimizer = new XElement("optimizer");
            root.Add(eOptimizer);

            Validation.Write(eOptimizer);
            Training.Write(eOptimizer);
            Fitness.Write(eOptimizer);
        }

    }

    [Serializable]
    public class NeuronNetwork
    {
        public NeuronNetworkLevel[] Layers { get; private set; }

        public float WeigthMultiplier { get; private set; } = 2.0F;

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

        public const int HISTORY_WINDOW_LENGTH = 27;

        private static int[] NeuronsConfig = new int[] {
            HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER + (TradingItemInitData.IS_RECURRENT ? DirectionTester.OUTPUT_NUMBER : 0),
            1 * (HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER)  + (TradingItemInitData.IS_RECURRENT ? DirectionTester.OUTPUT_NUMBER : 0),
            DirectionTester.OUTPUT_NUMBER,
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

            var eWeigthMultiplier = networkElement.Element("weigthMultiplier");
            WeigthMultiplier = (float)Convert.ToDouble(eWeigthMultiplier.Value);

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

            neuronNetwork.Add(new XElement("weigthMultiplier", WeigthMultiplier.ToString(CultureInfo.InvariantCulture)));
        }
    }

    [Serializable]
    public class TradingItemInitData : GeneticItemInitData
    {
        public const bool IS_RECURRENT = true;

        private NeuronNetwork _neuronNetwork;
        private int _historyWidowLength;

        private double _indicatorMutation = 0.999;
        private double _valuesMutation = 0.999;
        private double _otherValuesMutation = 0.999;


        public const int MAX_STOPLOSS = 100;
        private const int OFFSET = 0;
        public const int NUMBER_PARAMETERS = 2;

        public int OFFSET_ALPHA
        {
            get { return OFFSET + InputData.Indicators.IndicatorNumber; }
        }

        public int OFFSET_STOPLOSS
        {
            get { return OFFSET + InputData.Indicators.IndicatorNumber + 1; }
        }

        public int OFFSET_VALUES
        {
            get { return OFFSET + InputData.Indicators.IndicatorNumber + 2; }
        }

        public InputData InputData { get; } = new InputData();
        public Optimizer Optimizer { get; } = new Optimizer();

        public TradingItemInitData() : this(new NeuronNetwork())
        {
        }

        protected TradingItemInitData(NeuronNetwork neuronNetwork) : base(Indicators.INDICATOR_NUMBER + NUMBER_PARAMETERS + neuronNetwork.GetWeightsCount(), false)
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

        public void Read(XElement root)
        {
            var eItemInitData = root.Element("itemInitData");

            _neuronNetwork = new NeuronNetwork();
            _neuronNetwork.Read(eItemInitData);

            InputData.Read(eItemInitData);
            Optimizer.Read(eItemInitData);

            var eHistoryWidowLength = eItemInitData.Element("historyWidowLength");
            _historyWidowLength = int.Parse(eHistoryWidowLength.Value);

            _indicatorMutation = double.Parse(eItemInitData.Element("indicatorMutation").Value);
            _otherValuesMutation = double.Parse(eItemInitData.Element("otherValuesMutation").Value);
            _valuesMutation = double.Parse(eItemInitData.Element("valuesMutation").Value);

            _valuesNumber = _neuronNetwork.GetWeightsCount() + OFFSET_VALUES;
        }

        public void Write(XElement root)
        {
            var itemInitDataElement = new XElement("itemInitData");
            root.Add(itemInitDataElement);
            _neuronNetwork.Write(itemInitDataElement);

            InputData.Write(itemInitDataElement);
            Optimizer.Write(itemInitDataElement);

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
            int itemsCount, int surviveCount, int processorsCount)
        {
            TrainingData = trainingData;
            ValidationData = validationData;
            ItemInitData = itemInitData;
            CommandInitData = new GeneticCommandInitData(itemsCount, surviveCount, 3000000, processorsCount);
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

        public override double CalculateFitness(TradingItem item)
        {
            bool[] oldValues = new bool[DataObject.TrainingData.Indicators.Length];
            for (int i = 0; i < item.InitData.InputData.Indicators.IndicatorNumber; i++)
            {
                var index = (int)item.Values[i];
                if (oldValues[index])
                    return Double.MinValue;
                oldValues[index] = true;
            }


            var dll = GetNeuronDll(item);
            double validationZigZagCount;
            var validationResult = GetProfit(dll, item, DataObject.ValidationData, out validationZigZagCount);
            item.ValidationTesterResult = validationResult;

            if (DataObject.ItemInitData.Optimizer.Validation.IsFilterBadResult && FilterBadResult(validationResult))
                return Double.MinValue;

            double trainingZigZagCount;
            var trainingResult = GetProfit(dll, item, DataObject.TrainingData, out trainingZigZagCount);
            item.TrainingTesterResult = trainingResult;

            if (DataObject.ItemInitData.Optimizer.IsValidationPlusMinusRatioLessTraining)
            {
                if (validationResult.MinusCount > 0 && trainingResult.MinusCount > 0)
                {
                    if (((float) validationResult.PlusCount / validationResult.MinusCount) <
                        ((float) trainingResult.PlusCount / trainingResult.MinusCount))
                    {
                        return Double.MinValue;
                    }
                }
            }

            if (validationResult.Profit <= 0 || trainingResult.Profit <= 0)
            {
                return Double.MinValue;
            }

            if (DataObject.ItemInitData.Optimizer.Training.IsFilterBadResult && FilterBadResultBuySell(trainingResult))
            {
                return Double.MinValue;
            }

            if (DataObject.ItemInitData.Optimizer.Training.IsFilterBadResultBuySell && FilterBadResultBuySell(trainingResult))
            {
                return Double.MinValue;
            }

            


            if (DataObject.ItemInitData.Optimizer.Fitness.IsZigZag)
            {
                if (validationZigZagCount + trainingZigZagCount < 0)
                {
                    return validationZigZagCount + trainingZigZagCount;
                }
            }

            var fitness = 1.0;

            if (DataObject.ItemInitData.Optimizer.Fitness.IsProfit)
            {
                fitness *= (double) (validationResult.Profit + trainingResult.Profit);
            }

            if (DataObject.ItemInitData.Optimizer.Fitness.IsZigZag)
            {
                fitness *= (double) (validationZigZagCount + trainingZigZagCount);
            }

            if (DataObject.ItemInitData.Optimizer.Fitness.IsExpectedValue)
            {
                var m = (validationResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit) / validationResult.Orders.Count +
                         trainingResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit) / trainingResult.Orders.Count) / 2;
                fitness *= (double)m;
            }
            if (DataObject.ItemInitData.Optimizer.Fitness.IsPlusMinusOrdersRatio)
            {
                var pmRatio = ((double) (trainingResult.PlusCount + validationResult.PlusCount) /
                               ((trainingResult.MinusCount + validationResult.MinusCount) > 0 ? (trainingResult.MinusCount + validationResult.MinusCount) : 1) );
                fitness *= (double)pmRatio;
            }
            if (DataObject.ItemInitData.Optimizer.Fitness.IsPlusMinusEquityRatio)
            {
                var ecRation = ((double)(trainingResult.PlusEquityCount + validationResult.PlusEquityCount) /
                            ((trainingResult.MinusEquityCount + validationResult.MinusEquityCount) > 0 ? (trainingResult.MinusEquityCount + validationResult.MinusEquityCount) : 1));
                fitness *= (double)ecRation;
            }

            return fitness;
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

            if (FilterBadResultBuySell(testerResult))
                return true;

            return false;
        }


        public bool FilterBadResultBuySell(TesterResult testerResult)
        {
            if (testerResult.BuyCount > testerResult.SellCount)
            {
                if (testerResult.BuyCount / (float)testerResult.SellCount > 2)
                {
                    return true;
                }
            }
            else
            {
                if (testerResult.SellCount / (float)testerResult.BuyCount > 2)
                {
                    return true;
                }
            }
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

            var offset = item.InitData.OFFSET_VALUES;

            var v2 = item.InitData.NeuronNetwork.WeigthMultiplier;
            var v1 = v2 / 2F;

            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = values[offset] * v2 - v1;
                ++offset;
            }
        }


        //protected override void SortFitness()
        //{
        //    var list = (from item in _itemsArray
        //        orderby item.Fitness descending
        //        where item.Fitness > Double.MinValue
        //        select item).ToList();
        //    while (list.Count < DataObject.CommandInitData.ItemsCount)
        //    {
        //        var item = InternalCreateItem();
        //        FillValues(item);
        //        list.Add(item);
        //    }
        //    _itemsArray = list.ToArray();
        //}

        protected override void FillValues(TradingItem item)
        {
            var values = item.Values;
            var allIndexes = GetUniqueIndicatorIndexes();
            for (int i = 0; i < item.InitData.InputData.Indicators.IndicatorNumber; i++)
            {
                values[i] = allIndexes[i];
            }

            for (int j = item.InitData.OFFSET_ALPHA; j < DataObject.ItemInitData.ValuesNumber; j++)
            {
                var valueValue = item.CreateValue(Random);
                values[j] = valueValue;
            }
        }

        protected override void Mutation(TradingItem item)
        {
            if (string.IsNullOrEmpty(item.InitData.InputData.Indicators.PredefinedNames))
            {
                if (Random.NextDouble() > item.InitData.IndicatorMutation)
                {
                    var allIndexes = GetUniqueIndicatorIndexes();
                    for (int i = 0; i < item.InitData.InputData.Indicators.IndicatorNumber; i++)
                    {
                        item.Values[i] = allIndexes[i];
                    }
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

        private volatile object _syncPredefinedIndicatorsIndexes = new object();
        private int[] _predefinedIndicatorsIndexes = null;

        private int[] GetUniqueIndicatorIndexes()
        {
            var result = new int[DataObject.TrainingData.Indicators.Length];

            if (!string.IsNullOrEmpty(DataObject.ItemInitData.InputData.Indicators.PredefinedNames))
            {
                lock (_syncPredefinedIndicatorsIndexes)
                {
                    if (_predefinedIndicatorsIndexes == null)
                    {
                        var names = DataObject.ItemInitData.InputData.Indicators.PredefinedNames.Split(new[] {','},
                            StringSplitOptions.RemoveEmptyEntries);

                        if (names.Length != DataObject.ItemInitData.InputData.Indicators.IndicatorNumber)
                            throw new Exception("Invalid PredefinedNames " +
                                                DataObject.ItemInitData.InputData.Indicators.PredefinedNames);

                        for (int i = 0; i < names.Length; i++)
                        {
                            var name = names[i];
                            var search = new LearningDataIndicator.IndicaorSearch(name);
                            var index = DataObject.TrainingData.Indicators.ToList().FindIndex(search.Match);
                            if (index < 0)
                                throw new Exception("Invalid indicator name:" + name);
                            result[i] = index;
                        }
                        _predefinedIndicatorsIndexes = result;
                    }
                    return _predefinedIndicatorsIndexes;
                }
            }


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
