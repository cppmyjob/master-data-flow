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
using MasterDataFlow.Intelligence.Neuron.SimpleNeuron;
using MasterDataFlow.Trading.Configs;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Genetic
{

    [Serializable]
    public class LearningInputData :  IComparable
    {
        public class IndicatorSearch
        {
            private string _s;

            public IndicatorSearch(string s)
            {
                _s = s;
            }

            public bool Match(LearningInputData e)
            {
                return e.Name == _s;
            }
        }

        public string Name { get; set; }
        public float[] Values { get; set; }
        public DateTime[] Times { get; set; }

        //public void Normalize()
        //{
        //    var min = Values.Min();
        //    var max = Values.Max();
        //    var diff = max - min;
        //    var offset = diff * 20 / 100;
        //    min = min - offset;
        //    max = max + offset;

        //    diff = max - min;

        //    Values = Values.Select(t => (t - min) / diff ).ToArray();

        //}

        public int CompareTo(object o)
        {
            if (!(o is LearningInputData indicator))
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
        public LearningInputData[] Indicators { get; set; }
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
    public class InputDataSection
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
                IsFilterBadResultBuySell = configSection.Optimizer.Validation.IsFilterBadResultBuySell;
            }
        }


        public bool IsFilterBadResult { get; private set; } = true;
        public bool IsFilterBadResultBuySell { get; set; } = true;

        public void Read(XElement root)
        {
            var eValidation = root.Element("validation");
            if (eValidation == null)
                return;

            IsFilterBadResult = Convert.ToBoolean(eValidation.Element("isFilterBadResult").Value);

            if (eValidation.Element("isFilterBadResultBuySell") != null)
            {
                IsFilterBadResultBuySell = Convert.ToBoolean(eValidation.Element("isFilterBadResultBuySell").Value);
            }
        }

        public void Write(XElement root)
        {
            var eValidation = new XElement("validation");
            root.Add(eValidation);

            eValidation.Add(new XElement("isFilterBadResult", IsFilterBadResult.ToString(CultureInfo.InvariantCulture)));
            eValidation.Add(new XElement("isFilterBadResultBuySell", IsFilterBadResultBuySell.ToString(CultureInfo.InvariantCulture)));
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
        public bool IsFilterBadResultBuySell { get; set; } = true;

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
                ValidationPercent = configSection.Optimizer.Fitness.ValidationPercent;
            }
        }


        public bool IsExpectedValue { get; private set; } = true;
        public bool IsPlusMinusOrdersRatio { get; private set; } = true;
        public bool IsPlusMinusEquityRatio { get; private set; } = true;
        public bool IsProfit { get; private set; } = true;
        public bool IsZigZag { get; private set; } = true;

        public int ValidationPercent { get; private set; } = 0;


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

            if (eFitness.Element("validationPercent") != null)
                ValidationPercent = Convert.ToInt32(eFitness.Element("validationPercent").Value);
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
            eFitness.Add(new XElement("validationPercent", ValidationPercent.ToString(CultureInfo.InvariantCulture)));
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
            HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER + (TradingItemInitData.IS_RECURRENT ? TradingItemInitData.OUTPUT_NUMBER : 0),
            1 * (HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER)  + (TradingItemInitData.IS_RECURRENT ? TradingItemInitData.OUTPUT_NUMBER : 0),
            (1 * (HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER)  + (TradingItemInitData.IS_RECURRENT ? TradingItemInitData.OUTPUT_NUMBER : 0)) / 2,
            (1 * (HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER)  + (TradingItemInitData.IS_RECURRENT ? TradingItemInitData.OUTPUT_NUMBER : 0)) / 4,
            (1 * (HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER)  + (TradingItemInitData.IS_RECURRENT ? TradingItemInitData.OUTPUT_NUMBER : 0)) / 8,
            TradingItemInitData.OUTPUT_NUMBER,
        };

        //private static int[] NeuronsConfig = new int[] {
        //    HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER + (TradingItemInitData.IS_RECURRENT ? TradingItemInitData.OUTPUT_NUMBER : 0),
        //    (1 * (HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER)  + (TradingItemInitData.IS_RECURRENT ? TradingItemInitData.OUTPUT_NUMBER : 0)) * 1,
        //    (1 * (HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER)  + (TradingItemInitData.IS_RECURRENT ? TradingItemInitData.OUTPUT_NUMBER : 0)) * 1,
        //    (1 * (HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER)  + (TradingItemInitData.IS_RECURRENT ? TradingItemInitData.OUTPUT_NUMBER : 0)) / 4,
        //    (1 * (HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER)  + (TradingItemInitData.IS_RECURRENT ? TradingItemInitData.OUTPUT_NUMBER : 0)) / 4,
        //    (1 * (HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER)  + (TradingItemInitData.IS_RECURRENT ? TradingItemInitData.OUTPUT_NUMBER : 0)) / 8,
        //    (1 * (HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER)  + (TradingItemInitData.IS_RECURRENT ? TradingItemInitData.OUTPUT_NUMBER : 0)) / 8,
        //    TradingItemInitData.OUTPUT_NUMBER,
        //    TradingItemInitData.OUTPUT_NUMBER,
        //};

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

        public static Neuron CreateNeuronDll(NeuronNetwork neuronNetwork, TradingItem item)
        {
            var dll = CreateNeuronDll(neuronNetwork);
            dll.SetAlpha(item.Alpha);
            SetWeigths(dll, item);
            return dll;
        }

        public static Neuron CreateNeuronDll(NeuronNetwork neuronNetwork)
        {
            Neuron dll = new Neuron();
            float alpha = 1.5F;
            dll.NetworkCreate(alpha, neuronNetwork.GetNeuronsConfig());
            dll.CreateWeigths(neuronNetwork.GetWeightsCount());
            return dll;
        }

        public  static void SetWeigths(ISimpleNeuron neuron, TradingItem item)
        {
            var weights = neuron.GetWeigths();
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

    }

    [Serializable]
    public class TradingItemInitData : GeneticItemInitData
    {
        public const bool IS_RECURRENT = true;
        public const int OUTPUT_NUMBER = 3;


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

        public InputDataSection InputData { get; } = new InputDataSection();
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

    public interface IFitness
    {
        double Fitness { get; set; }
        double FitnessZigZag { get; set; }
        double FitnessExpectedValue { get; set; }
        double FitnessProfit { get; set; }
        double FitnessPlusMinusOrdersRatio { get; set; }
        double FitnessPlusMinusEquityRatio { get; set; }
    }

    public class FitnessData : IFitness {
        public double Fitness { get; set; }
        public double FitnessZigZag { get; set; }
        public double FitnessExpectedValue { get; set; }
        public double FitnessProfit { get; set; }
        public double FitnessPlusMinusOrdersRatio { get; set; }
        public double FitnessPlusMinusEquityRatio { get; set; }
    }

    [Serializable]
    public class TradingItem : GeneticFloatItem<TradingItemInitData>, IFitness
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

        public double FitnessZigZag { get; set; }
        public double FitnessExpectedValue { get; set; }
        public double FitnessProfit { get; set; }
        public double FitnessPlusMinusOrdersRatio { get; set; }
        public double FitnessPlusMinusEquityRatio { get; set; }
    }


    public class TradingCommand : GeneticFloatCommand<TradingDataObject, TradingItemInitData, TradingItem>
    {
        private Dictionary<int, ISimpleNeuron> _neurons = new Dictionary<int, ISimpleNeuron>();

        protected override TradingItem CreateItem(TradingItemInitData initData)
        {
            return new TradingItem(initData);
        }

        private IFitness GetFitness(TesterResult testerResult, double zigZagCount)
        {
            var result = new FitnessData();

            // ExpectedValue
            {
                // https://www.mql5.com/ru/blogs/post/651765
                var sdelki = (double)(testerResult.Orders.Count);
                if (sdelki > 0)
                {
                    var pplus = (testerResult.PlusCount) / sdelki;
                    var vplus = (double)(testerResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit)) / sdelki;
                    var pminus = (testerResult.MinusCount) / sdelki;
                    var vminus = (double)Math.Abs(testerResult.Orders.Where(t => t.Profit < 0).Sum(t => t.Profit)) /
                                 sdelki;
                    var m = pplus * vplus - pminus * vminus;

                    if (m < 0)
                        m = 0.000001;
                    result.FitnessExpectedValue = NormalizeValue(m);
                }
                else
                    result.FitnessExpectedValue = 0;
            }

            // Profit
            {
                result.FitnessProfit = NormalizeValue((double)(testerResult.Profit));
            }

            // ZigZag
            {
                if (zigZagCount < 0)
                    zigZagCount = 1 / Math.Abs(zigZagCount);

                result.FitnessZigZag = NormalizeValue(zigZagCount);
            }

            // PlusMinusOrdersRatio
            {
                var pmRatio = ((double)(testerResult.PlusCount) /
                               ((testerResult.MinusCount) > 0 ? (testerResult.MinusCount) : 1));
                result.FitnessPlusMinusOrdersRatio = NormalizeValue(pmRatio);
            }

            // PlusMinusEquityRatio
            {
                var ecRation = ((double)(testerResult.PlusEquityCount) /
                                ((testerResult.MinusEquityCount) > 0 ? (testerResult.MinusEquityCount) : 1));
                result.FitnessPlusMinusEquityRatio = NormalizeValue(ecRation);
            }

            var fitness = 1.0;


            if (DataObject.ItemInitData.Optimizer.Fitness.IsZigZag)
            {
                fitness *= result.FitnessZigZag;
            }

            if (DataObject.ItemInitData.Optimizer.Fitness.IsExpectedValue)
            {
                fitness *= result.FitnessExpectedValue;
            }

            if (DataObject.ItemInitData.Optimizer.Fitness.IsProfit)
            {
                fitness *= result.FitnessProfit;
            }

            if (DataObject.ItemInitData.Optimizer.Fitness.IsPlusMinusOrdersRatio)
            {
                fitness *= result.FitnessPlusMinusOrdersRatio;
            }

            if (DataObject.ItemInitData.Optimizer.Fitness.IsPlusMinusEquityRatio)
            {
                fitness *= result.FitnessPlusMinusEquityRatio;
            }

            result.Fitness = fitness;

            return result;
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

            int trainingZigZagCount;
            var trainingResult = GetProfit(dll, item, DataObject.TrainingData, out trainingZigZagCount);
            item.TrainingTesterResult = trainingResult;

            if (trainingResult.Profit <= 0)
            {
                return Double.MinValue;
            }

            if (DataObject.ItemInitData.Optimizer.Training.IsFilterBadResult && FilterBadResult(trainingResult))
            {
                return Double.MinValue;
            }

            if (DataObject.ItemInitData.Optimizer.Training.IsFilterBadResultBuySell && FilterBadResultBuySell(trainingResult))
            {
                return Double.MinValue;
            }

            int validationZigZagCount;
            var validationResult = GetProfit(dll, item, DataObject.ValidationData, out validationZigZagCount);
            item.ValidationTesterResult = validationResult;

            if (validationResult.Profit <= 0)
            {
                return Double.MinValue;
            }

            if (DataObject.ItemInitData.Optimizer.Validation.IsFilterBadResult && FilterBadResult(validationResult))
                return Double.MinValue;

            if (DataObject.ItemInitData.Optimizer.Validation.IsFilterBadResultBuySell && FilterBadResultBuySell(validationResult))
            {
                return Double.MinValue;
            }

            if (DataObject.ItemInitData.Optimizer.IsValidationPlusMinusRatioLessTraining)
            {
                if (validationResult.MinusCount > 0 && trainingResult.MinusCount > 0)
                {
                    if (((float)validationResult.PlusCount / validationResult.MinusCount) <
                        ((float)trainingResult.PlusCount / trainingResult.MinusCount))
                    {
                        return Double.MinValue;
                    }
                }
            }


            var trainingFitness = GetFitness(trainingResult, trainingZigZagCount);

            var validationFitness = GetFitness(validationResult, validationZigZagCount);

            item.FitnessZigZag = trainingFitness.FitnessZigZag + validationFitness.FitnessZigZag;
            item.FitnessExpectedValue = trainingFitness.FitnessExpectedValue + validationFitness.FitnessExpectedValue;
            item.FitnessProfit = trainingFitness.FitnessProfit + validationFitness.FitnessProfit;
            item.FitnessPlusMinusOrdersRatio = trainingFitness.FitnessPlusMinusOrdersRatio + validationFitness.FitnessPlusMinusOrdersRatio;
            item.FitnessPlusMinusEquityRatio = trainingFitness.FitnessPlusMinusEquityRatio + validationFitness.FitnessPlusMinusEquityRatio;

            var validationPercent = DataObject.ItemInitData.Optimizer.Fitness.ValidationPercent;

            if (validationPercent > 0)
            {
                var percentMin = 1 - (validationPercent / (double)100);
                var percentMax = 1 + (validationPercent / (double)100);

                var min = trainingFitness.Fitness * percentMin;
                var max = trainingFitness.Fitness * percentMax;
                if (!(min <= validationFitness.Fitness && validationFitness.Fitness <= max))
                {
                    return Double.MinValue;
                }
            }

            return trainingFitness.Fitness + validationFitness.Fitness;
        }

        /*
         *

                    // ZigZag
                    {
                        item.FitnessZigZag = (validationZigZagCount + trainingZigZagCount);
                        if (item.FitnessZigZag < 0)
                            item.FitnessZigZag = 1 / Math.Abs(item.FitnessZigZag);
                    }


                    // ExpectedValue
                    {
                        // https://www.mql5.com/ru/blogs/post/651765
                        //var m = (validationResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit) / validationResult.Orders.Count +
                        //         trainingResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit) / trainingResult.Orders.Count) / 2;
                        var sdelki = (double)(validationResult.Orders.Count + trainingResult.Orders.Count);
                        var pplus = (validationResult.PlusCount + trainingResult.PlusCount)
                                    / sdelki;
                        var vplus = (double)(validationResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit) +
                                     trainingResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit))
                                    / sdelki;
                        var pminus = (validationResult.MinusCount + trainingResult.MinusCount)
                                    / sdelki;
                        var vminus = (double)Math.Abs(validationResult.Orders.Where(t => t.Profit < 0).Sum(t => t.Profit) +
                                     trainingResult.Orders.Where(t => t.Profit < 0).Sum(t => t.Profit))
                                    / sdelki;
                        var m = pplus * vplus - pminus * vminus;

                        item.FitnessExpectedValue = m;
                        if (item.FitnessExpectedValue < 0)
                            item.FitnessExpectedValue = 0.000001;
                    }

                    // Profit
                    {
                        item.FitnessProfit = (double)(validationResult.Profit + trainingResult.Profit);
                    }

                    // PlusMinusOrdersRatio
                    {
                        var pmRatio = ((double)(trainingResult.PlusCount + validationResult.PlusCount) /
                                       ((trainingResult.MinusCount + validationResult.MinusCount) > 0 ? (trainingResult.MinusCount + validationResult.MinusCount) : 1));
                        item.FitnessPlusMinusOrdersRatio = pmRatio;
                    }

                    // PlusMinusEquityRatio
                    {
                        var ecRation = ((double)(trainingResult.PlusEquityCount + validationResult.PlusEquityCount) /
                                    ((trainingResult.MinusEquityCount + validationResult.MinusEquityCount) > 0 ? (trainingResult.MinusEquityCount + validationResult.MinusEquityCount) : 1));
                        item.FitnessPlusMinusEquityRatio = ecRation;
                    }

                    var fitness = 1.0;


                    if (DataObject.ItemInitData.Optimizer.Fitness.IsZigZag)
                    {
                        fitness *= item.FitnessZigZag;
                    }

                    if (DataObject.ItemInitData.Optimizer.Fitness.IsExpectedValue)
                    {
                        fitness *= item.FitnessExpectedValue;
                    }

                    if (DataObject.ItemInitData.Optimizer.Fitness.IsProfit)
                    {
                        fitness *= item.FitnessProfit;
                    }

                    if (DataObject.ItemInitData.Optimizer.Fitness.IsPlusMinusOrdersRatio)
                    {
                        fitness *= item.FitnessPlusMinusOrdersRatio;
                    }

                    if (DataObject.ItemInitData.Optimizer.Fitness.IsPlusMinusEquityRatio)
                    {
                        fitness *= item.FitnessPlusMinusEquityRatio;
                    }

                    return fitness;


         *
         */









        //public override double CalculateFitness(TradingItem item)
        //{
        //    bool[] oldValues = new bool[DataObject.TrainingData.Indicators.Length];
        //    for (int i = 0; i < item.InitData.InputData.Indicators.IndicatorNumber; i++)
        //    {
        //        var index = (int)item.Values[i];
        //        if (oldValues[index])
        //            return Double.MinValue;
        //        oldValues[index] = true;
        //    }


        //    var dll = GetNeuronDll(item);
        //    int validationZigZagCount;
        //    var validationResult = GetProfit(dll, item, DataObject.ValidationData, out validationZigZagCount);
        //    item.ValidationTesterResult = validationResult;

        //    if (DataObject.ItemInitData.Optimizer.Validation.IsFilterBadResult && FilterBadResult(validationResult))
        //        return Double.MinValue;

        //    int trainingZigZagCount;
        //    var trainingResult = GetProfit(dll, item, DataObject.TrainingData, out trainingZigZagCount);
        //    item.TrainingTesterResult = trainingResult;

        //    if (DataObject.ItemInitData.Optimizer.IsValidationPlusMinusRatioLessTraining)
        //    {
        //        if (validationResult.MinusCount > 0 && trainingResult.MinusCount > 0)
        //        {
        //            if (((float)validationResult.PlusCount / validationResult.MinusCount) <
        //                ((float)trainingResult.PlusCount / trainingResult.MinusCount))
        //            {
        //                return Double.MinValue;
        //            }
        //        }
        //    }

        //    if (validationResult.Profit <= 0 || trainingResult.Profit <= 0)
        //    {
        //        return Double.MinValue;
        //    }

        //    if (DataObject.ItemInitData.Optimizer.Training.IsFilterBadResult && FilterBadResultBuySell(trainingResult))
        //    {
        //        return Double.MinValue;
        //    }

        //    if (DataObject.ItemInitData.Optimizer.Training.IsFilterBadResultBuySell && FilterBadResultBuySell(trainingResult))
        //    {
        //        return Double.MinValue;
        //    }

        //    //if (DataObject.ItemInitData.Optimizer.Fitness.IsZigZag)
        //    //{
        //    //    if (validationZigZagCount + trainingZigZagCount < 0)
        //    //    {
        //    //        return validationZigZagCount + trainingZigZagCount;
        //    //    }
        //    //}

        //    // ZigZag
        //    {
        //        item.FitnessZigZag = (validationZigZagCount + trainingZigZagCount);
        //        if (item.FitnessZigZag < 0)
        //            item.FitnessZigZag = 1 / Math.Abs(item.FitnessZigZag);
        //        item.FitnessZigZag = NormalizeValue(item.FitnessZigZag);
        //    }


        //    // ExpectedValue
        //    {
        //        // https://www.mql5.com/ru/blogs/post/651765
        //        //var m = (validationResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit) / validationResult.Orders.Count +
        //        //         trainingResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit) / trainingResult.Orders.Count) / 2;
        //        var sdelki = (double)(validationResult.Orders.Count + trainingResult.Orders.Count);
        //        var pplus = (validationResult.PlusCount + trainingResult.PlusCount)
        //                    / sdelki;
        //        var vplus = (double)(validationResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit) +
        //                     trainingResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit))
        //                    / sdelki;
        //        var pminus = (validationResult.MinusCount + trainingResult.MinusCount)
        //                    / sdelki;
        //        var vminus = (double)Math.Abs(validationResult.Orders.Where(t => t.Profit < 0).Sum(t => t.Profit) +
        //                     trainingResult.Orders.Where(t => t.Profit < 0).Sum(t => t.Profit))
        //                    / sdelki;
        //        var m = pplus * vplus - pminus * vminus;

        //        item.FitnessExpectedValue = m;
        //        if (item.FitnessExpectedValue < 0)
        //            item.FitnessExpectedValue = 0.000001;
        //        item.FitnessExpectedValue = NormalizeValue(item.FitnessExpectedValue);
        //    }

        //    // Profit
        //    {
        //        item.FitnessProfit = NormalizeValue((double)(validationResult.Profit + trainingResult.Profit));
        //    }

        //    // PlusMinusOrdersRatio
        //    {
        //        var pmRatio = ((double)(trainingResult.PlusCount + validationResult.PlusCount) /
        //                       ((trainingResult.MinusCount + validationResult.MinusCount) > 0 ? (trainingResult.MinusCount + validationResult.MinusCount) : 1));
        //        item.FitnessPlusMinusOrdersRatio = NormalizeValue(pmRatio);
        //    }

        //    // PlusMinusEquityRatio
        //    {
        //        var ecRation = ((double)(trainingResult.PlusEquityCount + validationResult.PlusEquityCount) /
        //                    ((trainingResult.MinusEquityCount + validationResult.MinusEquityCount) > 0 ? (trainingResult.MinusEquityCount + validationResult.MinusEquityCount) : 1));
        //        item.FitnessPlusMinusEquityRatio = NormalizeValue(ecRation);
        //    }

        //    var fitness = 1.0;


        //    if (DataObject.ItemInitData.Optimizer.Fitness.IsZigZag)
        //    {
        //        fitness *= item.FitnessZigZag;
        //    }

        //    if (DataObject.ItemInitData.Optimizer.Fitness.IsExpectedValue)
        //    {
        //        fitness *= item.FitnessExpectedValue;
        //    }

        //    if (DataObject.ItemInitData.Optimizer.Fitness.IsProfit)
        //    {
        //        fitness *= item.FitnessProfit;
        //    }

        //    if (DataObject.ItemInitData.Optimizer.Fitness.IsPlusMinusOrdersRatio)
        //    {
        //        fitness *= item.FitnessPlusMinusOrdersRatio;
        //    }

        //    if (DataObject.ItemInitData.Optimizer.Fitness.IsPlusMinusEquityRatio)
        //    {
        //        fitness *= item.FitnessPlusMinusEquityRatio;
        //    }

        //    return fitness;
        //}

        private double NormalizeValue(double value)
        {
            if (value <= 0)
                return 0;
            return Math.Log(value + 1);
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

            //if (FilterBadResultBuySell(testerResult))
            //    return true;

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

        private TesterResult GetProfit(ISimpleNeuron neuron, TradingItem item, LearningData learningData, out int zigZagCount)
        {
            var tester = new DirectionTester(neuron, item, learningData);
            TesterResult result = tester.Run();
            zigZagCount = tester.ZigZagCount;
            return result;
        }

        private ISimpleNeuron GetNeuronDll(TradingItem item)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            ISimpleNeuron neuron;
            lock (_neurons) // TODO improve
            {
                if (!_neurons.TryGetValue(threadId, out neuron))
                {
                    neuron = NeuronNetwork.CreateNeuronDll(DataObject.ItemInitData.NeuronNetwork);
                    _neurons.Add(threadId, neuron);
                }
            }
            neuron.SetAlpha(item.Alpha);
            NeuronNetwork.SetWeigths(neuron, item);
            return neuron;
        }


        protected override void SortFitness()
        {
            var partLength = DataObject.CommandInitData.SurviveCount / 6;

            var list = new List<TradingItem>(DataObject.CommandInitData.SurviveCount);

            var tempList = (from item in _itemsArray
                orderby item.Fitness descending
                select item).ToList();
            list.AddRange(tempList.Take(partLength));
            tempList = tempList.Skip(partLength).ToList();

            tempList = (from item in tempList
                        orderby item.FitnessExpectedValue descending
                select item).ToList();
            list.AddRange(tempList.Take(partLength));
            tempList = tempList.Skip(partLength).ToList();

            tempList = (from item in tempList
                orderby item.FitnessZigZag descending
                select item).ToList();
            list.AddRange(tempList.Take(partLength));
            tempList = tempList.Skip(partLength).ToList();

            tempList = (from item in tempList
                orderby item.FitnessProfit descending
                select item).ToList();
            list.AddRange(tempList.Take(partLength));
            tempList = tempList.Skip(partLength).ToList();

            tempList = (from item in tempList
                orderby item.FitnessPlusMinusOrdersRatio descending
                select item).ToList();
            list.AddRange(tempList.Take(partLength));
            tempList = tempList.Skip(partLength).ToList();

            tempList = (from item in tempList
                orderby item.FitnessExpectedValue descending
                select item).ToList();

            list.AddRange(tempList);

            while (list.Count < DataObject.CommandInitData.ItemsCount)
            {
                var item = InternalCreateItem();
                FillValues(item);
                list.Add(item);
            }
            _itemsArray = list.ToArray();
        }

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
                            var search = new LearningInputData.IndicatorSearch(name);
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
