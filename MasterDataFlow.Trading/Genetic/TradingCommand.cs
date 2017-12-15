using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class LearningData
    {
        public Bar[] Prices { get; set; }
        public LearningDataIndicator[] Indicators { get; set; }
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

        public int[] GetNeuronsConfig()
        {
            return Layers.Select(t => t.Length).ToArray();
        }
    }


    [Serializable]
    public class TradingItemInitData : GeneticItemInitData
    {
        public TradingItemInitData()
        {
        }

        public TradingItemInitData(int count, bool isAddHistory = false) : base(count, isAddHistory)
        {
        }
    }



    [Serializable]
    public class TradingDataObject : GeneticFloatDataObject<TradingItemInitData, TradingItem>
    {
        public LearningData TrainingData { get; }
        public LearningData ValidationData { get; }
        public NeuronNetwork DefaultNeuronNetwork { get; private set; }

        //public TradingDataObject(NeuronNetwork neuronNetwork, LearningData trainingData, LearningData validationData,
        //    int itemsCount, int surviveCount)
        //{
        //    DefaultNeuronNetwork = neuronNetwork;
        //    TrainingData = trainingData;
        //    ValidationData = validationData;
        //    RepeatCount = 10;
        //    CellInitData = new GeneticInitData(itemsCount, surviveCount, 
        //        TradingCommand.GetWeightsCount(this) + TradingItem.OFFSET_VALUES);
        //}

        public TradingDataObject(LearningData trainingData, LearningData validationData,
            int itemsCount, int surviveCount)
        {
            TrainingData = trainingData;
            ValidationData = validationData;
            CommandInitData = new GeneticCommandInitData(itemsCount, surviveCount, 3000000);
            SetDefaultNeuronNetwork(new NeuronNetwork
                                    {
                                        Layers = NeuronsConfig.Select(t => new NeuronNetworkLevel() { Length = t })
                                            .ToArray(),
                                    });
        }


        public void SetDefaultNeuronNetwork(NeuronNetwork neuronNetwork)
        {
            DefaultNeuronNetwork = neuronNetwork;
            //CommandInitData.SetValuesCount(TradingCommand.GetWeightsCount(this) + TradingItem.OFFSET_VALUES);
        }

        private static int[] NeuronsConfig = new int[] {
            TradingItem.HISTORY_WINDOW_LENGTH * TradingItem.INDICATOR_NUMBER,
            1 * (TradingItem.HISTORY_WINDOW_LENGTH * TradingItem.INDICATOR_NUMBER),
            2 * (TradingItem.HISTORY_WINDOW_LENGTH * TradingItem.INDICATOR_NUMBER),
            1 * (TradingItem.HISTORY_WINDOW_LENGTH * TradingItem.INDICATOR_NUMBER),
            2,
        };
    }

    [Serializable]
    public class TradingItem : GeneticFloatItem<TradingItemInitData>
    {
        internal const int HISTORY_WINDOW_LENGTH = 24;
        private int? _historyWidowLength;

        public const int INDICATOR_NUMBER = 3;
        public const int ALPHA_NUMBER = 1;
        public const int STOPLOSS_NUMBER = 1;

        public const int MAX_STOPLOSS = 100;

        public const int OFFSET_INDICATOR = 0;
        public const int OFFSET_ALPHA = OFFSET_INDICATOR + INDICATOR_NUMBER;
        public const int OFFSET_STOPLOSS = OFFSET_INDICATOR + INDICATOR_NUMBER + 1;
        public const int OFFSET_VALUES = OFFSET_INDICATOR + INDICATOR_NUMBER + 2;

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
                return Values[OFFSET_ALPHA] * 5;
            }
        }

        public float StopLoss
        {
            get { return Values[OFFSET_STOPLOSS] * MAX_STOPLOSS; }
        }

        public static int GetHistoryWidowLength(TradingItem item)
        {
            if (item != null)
            {
                if (item._historyWidowLength.HasValue)
                    return item._historyWidowLength.Value;
            }
            return HISTORY_WINDOW_LENGTH;
        }

        public void SetHistoryWidowLength(int value)
        {
            _historyWidowLength = value;
        }

        public TesterResult ValidationTesterResult { get; set; }

        public TesterResult TrainingTesterResult { get; set; }
    }


    public class TradingCommand : GeneticFloatCommand<TradingDataObject, TradingItemInitData, TradingItem>
    {
        private GeneticNeuronDLL1 _dll;

        protected override TradingItem CreateItem(TradingItemInitData initData)
        {
            return new TradingItem(initData);
        }

        public override double CalculateFitness(TradingItem item, int processor)
        {
            bool[] oldValues = new bool[DataObject.TrainingData.Indicators.Length];
            for (int i = 0; i < TradingItem.INDICATOR_NUMBER; i++)
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

            var validationResult = GetProfit(dll, item, DataObject.ValidationData);
            item.ValidationTesterResult = validationResult;

            if (FilterBadResult(validationResult))
                return Double.MinValue;

            var trainingResult = GetProfit(dll, item, DataObject.TrainingData);
            item.TrainingTesterResult = trainingResult;

            if (validationResult.MinusCount > 0 && trainingResult.MinusCount > 0)
            {
                if (((float)validationResult.PlusCount / validationResult.MinusCount) <
                    ((float)trainingResult.PlusCount / trainingResult.MinusCount))
                {
                    return Double.MinValue;
                }
            }

            if (trainingResult.Profit <= 0)
            {
                return Double.MinValue;
            }


            if (validationResult.MinusCount == 0 || trainingResult.MinusCount == 0)
                return Double.MinValue;

            //if (FilterBadResult(trainingResult))
            //    return Double.MinValue;

            //var m = 1m;

            var m = (validationResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit) +
                     trainingResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit)) /
                    (validationResult.Orders.Count + trainingResult.Orders.Count);

            return (double)(validationResult.Profit + trainingResult.Profit)
                   * (double) (TradingItem.MAX_STOPLOSS - item.StopLoss)
                   //* (double)(trainingResult.OrderCount + validationResult.OrderCount)
                   * (double)(trainingResult.PlusCount - trainingResult.MinusCount + validationResult.PlusCount - validationResult.MinusCount)
                   * (double)((double)(trainingResult.PlusCount + validationResult.PlusCount ) / (trainingResult.MinusCount + validationResult.MinusCount))
                   * (double)m;
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

            if (testerResult.BuyCount > testerResult.SellCount)
            {
                if (testerResult.BuyCount / (float) testerResult.SellCount > 2)
                {
                        return true;
                }
            }
            else
            {
                if (testerResult.SellCount / (float) testerResult.BuyCount > 2)
                {
                        return true;
                }
            }
            return false;
        }

        private TesterResult GetProfit(GeneticNeuronDLL1 dll, TradingItem item, LearningData learningData)
        {
            var tester = new DirectionTester(dll, item, learningData);
            TesterResult result = tester.Run();
            return result;
        }

        private GeneticNeuronDLL1 GetNeuronDll(TradingItem item)
        {
            if (_dll == null)
                _dll = CreateNeuronDll(DataObject);
            _dll.SetAlpha(item.Alpha);
            SetWeigths(_dll, item);
            return _dll;
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
            dll.NetworkCreate(alpha, dataObject.DefaultNeuronNetwork.GetNeuronsConfig());
            dll.CreateWeigths(GetWeightsCount(dataObject));
            return dll;
        }

        public static int GetWeightsCount(TradingDataObject dataObject)
        {
            int[] neurons = dataObject.DefaultNeuronNetwork.GetNeuronsConfig();
            int result = 0;
            for (int i = 1; i < neurons.Length; i++)
            {
                result += neurons[i - 1] * neurons[i];
            }
            return result;
        }


        private static void SetWeigths(GeneticNeuronDLL1 dll, TradingItem item)
        {
            var weights = dll.GetWeigths();
            var values = item.Values;

            //Parallel.For(0, weights.Length, i => weights[i] = values[i + TradingItem.OFFSET_VALUES] * 100.0F - 50.0F);

            var offset = TradingItem.OFFSET_VALUES;
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = values[offset] * 100.0F - 50.0F;
                //weights[i] = values[offset] * 2.0 - 1.0;
                ++offset;
            }
        }

        protected override void FillValues(TradingItem item)
        {
            var values = item.Values;
            var allIndexes = GetUniqueIndicatorIndexes();
            for (int i = 0; i < TradingItem.INDICATOR_NUMBER; i++)
            {
                values[i] = allIndexes[i];
            }

            for (int j = TradingItem.OFFSET_ALPHA; j < DataObject.ItemInitData.Count; j++)
            {
                var valueValue = item.CreateValue(Random);
                values[j] = valueValue;
            }
        }

        protected override void Mutation(TradingItem item)
        {
            if (Random.NextDouble() > 0.999)
            {
                var allIndexes = GetUniqueIndicatorIndexes();
                for (int i = 0; i < TradingItem.INDICATOR_NUMBER; i++)
                {
                    item.Values[i] = allIndexes[i];
                }
            }

            for (int i = TradingItem.OFFSET_ALPHA; i < item.Values.Length; i++)
            {
                if (Random.NextDouble() > 0.999)
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

            for (int i = 0; i < TradingItem.OFFSET_VALUES; i++)
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

            int[] neurons = DataObject.DefaultNeuronNetwork.GetNeuronsConfig();
            var offset = TradingItem.OFFSET_VALUES;
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
