using System;
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
    public class TradingDataObject : GeneticFloatDataObject<TradingItem>
    {
        public LearningData TrainingData { get; }
        public LearningData ValidationData { get; }

        public TradingDataObject(LearningData trainingData, LearningData validationData,
            int itemsCount, int surviveCount)
        {
            TrainingData = trainingData;
            ValidationData = validationData;
            RepeatCount = 10;
            CellInitData = new GeneticInitData(itemsCount, surviveCount, 
                TradingCommand.GetWeightsCount() + TradingItem.OFFSET_VALUES);
        }

    }

    [Serializable]
    public class TradingItem : GeneticFloatItem
    {
        public const int HISTORY_WINDOW_LENGTH = 24;

        public const int INDICATOR_NUMBER = 3;
        public const int ALPHA_NUMBER = 1;
        public const int STOPLOSS_NUMBER = 1;

        public const int OFFSET_INDICATOR = 0;
        public const int OFFSET_ALPHA = OFFSET_INDICATOR + INDICATOR_NUMBER;
        public const int OFFSET_STOPLOSS = OFFSET_INDICATOR + INDICATOR_NUMBER + 1;
        public const int OFFSET_VALUES = OFFSET_INDICATOR + INDICATOR_NUMBER + 2;

        public TradingItem(GeneticItemInitData initData) : base(initData)
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
            get { return Values[OFFSET_STOPLOSS] * 100; }
        }

        public TesterResult ValidationTesterResult { get; set; }

        public TesterResult TrainingTesterResult { get; set; }
    }


    public class TradingCommand : GeneticFloatCommand<TradingDataObject, TradingItem>
    {
        private GeneticNeuronDLL1 _dll;

        protected override TradingItem CreateItem(GeneticItemInitData initData)
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

            //if (FilterBadResult(trainingResult))
            //    return Double.MinValue;

            var m = 1m;

            //var m = (validationResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit) +
            //         trainingResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit)) /
            //        (validationResult.Orders.Count + trainingResult.Orders.Count);

            return (double)(validationResult.Profit + trainingResult.Profit)
                   * (double)(trainingResult.OrderCount + validationResult.OrderCount)
                   * (double)(trainingResult.PlusCount - trainingResult.MinusCount + validationResult.PlusCount - validationResult.MinusCount)
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
            if ((float) testerResult.MinusCount / (testerResult.PlusCount + testerResult.MinusCount) > 0.7)
            {
                    return true;
            }

            if (testerResult.BuyCount == 0 || testerResult.SellCount == 0)
            {
                    return true;
            }

            if (testerResult.BuyCount > testerResult.SellCount)
            {
                if (testerResult.BuyCount / (float) testerResult.SellCount > 4)
                {
                        return true;
                }
            }
            else
            {
                if (testerResult.SellCount / (float) testerResult.BuyCount > 4)
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
            dll.NetworkCreate(alpha, GetNeuronsConfig());
            dll.CreateWeigths(GetWeightsCount());
            return dll;
        }

        private static int[] NeuronsConfig =  new int[] {
            TradingItem.HISTORY_WINDOW_LENGTH * TradingItem.INDICATOR_NUMBER,
            1 * (TradingItem.HISTORY_WINDOW_LENGTH * TradingItem.INDICATOR_NUMBER),
            2 * (TradingItem.HISTORY_WINDOW_LENGTH * TradingItem.INDICATOR_NUMBER),
            1 * (TradingItem.HISTORY_WINDOW_LENGTH * TradingItem.INDICATOR_NUMBER),
            2,
        };

        public static int[] GetNeuronsConfig()
        {
            return NeuronsConfig;
        }

        public static int GetWeightsCount()
        {
            int[] neurons = GetNeuronsConfig();
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

            for (int j = TradingItem.OFFSET_ALPHA; j < DataObject.CellInitData.ValuesCount; j++)
            {
                var valueValue = item.CreateValue(Random);
                values[j] = valueValue;
            }
        }

        //protected override void Mutation(TradingItem item)
        //{
        //    if (Random.NextDouble() > 0.999)
        //    {
        //        var allIndexes = GetUniqueIndicatorIndexes();
        //        for (int i = 0; i < TradingItem.INDICATOR_NUMBER; i++)
        //        {
        //            item.Values[i] = allIndexes[i];
        //        }
        //    }

        //    for (int i = TradingItem.OFFSET_ALPHA; i < item.Values.Length; i++)
        //    {
        //        if (Random.NextDouble() > 0.999)
        //        {
        //            var valueValue = item.CreateValue(Random);
        //            item.Values[i] = valueValue;
        //        }
        //    }
        //}

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

            int[] neurons = GetNeuronsConfig();
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
