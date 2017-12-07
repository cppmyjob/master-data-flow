using System;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Intelligence.Neuron;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Genetic
{

    public class LearningData
    {
        public Bar[] Prices { get; set; }
        public float[][] Indicators { get; set; }
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
            CellInitData = new GeneticInitData(itemsCount, surviveCount, TradingCommand.GetWeightsCount());
        }

    }

    [Serializable]
    public class TradingItem : GeneticFloatItem
    {
        // Сколько баров будем проверять.
        public const int BAR_COUNT = 48;

        public const int INDICATOR_NUMBER = 5;
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

        public float Alpha
        {
            get
            {
                return Values[OFFSET_ALPHA] * 5;
            }
        }

        public int StopLoss
        {
            get { return (int)(Values[OFFSET_STOPLOSS] * 100); }
        }

        public FxTesterResult ValidationTesterResult { get; set; }

        public FxTesterResult TrainingTesterResult { get; set; }
    }


    public class TradingCommand : GeneticFloatCommand<TradingDataObject, TradingItem>
    {

        private GeneticNeuronDLL1 _dll;

        private GeneticNeuronDLL1 CreateNeuronDll()
        {
            GeneticNeuronDLL1 dll = new GeneticNeuronDLL1();
            float alpha = 1.5F;
            dll.NetworkCreate(alpha, GetNeuronsConfig());
            dll.CreateWeigths(GetWeightsCount());
            return dll;
        }

        public static int[] GetNeuronsConfig()
        {
            return new int[] {
                                 TradingItem.BAR_COUNT * TradingItem.INDICATOR_NUMBER,
                                 1 * (TradingItem.BAR_COUNT * TradingItem.INDICATOR_NUMBER),
                                 1 * (TradingItem.BAR_COUNT * TradingItem.INDICATOR_NUMBER), 
                                 2,
                             };
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


        protected override TradingItem CreateItem(GeneticItemInitData initData)
        {
            return new TradingItem(initData);
        }

        public override double CalculateFitness(TradingItem item, int processor)
        {

            if (item.StopLoss < 30 || item.StopLoss > 100)
            {
                return Double.MinValue;
            }

            var dll = GetNeuronDll(item);


            var validationResult = GetProfit(dll, item, DataObject.ValidationData);
            item.ValidationTesterResult = validationResult;
            if (validationResult.Profit <= 0)
            {
                return Double.MinValue;
            }
            if (validationResult.PlusCount < validationResult.MinusCount)
            {
                return Double.MinValue;
            }
            if (validationResult.BuyCount == 0 || validationResult.SellCount == 0)
            {
                return Double.MinValue;
            }

            var trainingResult = GetProfit(dll, item, DataObject.TrainingData);
            item.TrainingTesterResult = trainingResult;

            if (trainingResult.PlusCount < trainingResult.MinusCount)
            {
                return Double.MinValue;
            }
            if (validationResult.BuyCount == 0 || validationResult.SellCount == 0)
            {
                return Double.MinValue;
            }
            double buysell;
            if (validationResult.BuyCount > validationResult.SellCount)
                buysell = validationResult.SellCount / validationResult.BuyCount;
            else
                buysell = validationResult.BuyCount / validationResult.SellCount;


            return (validationResult.Profit + trainingResult.Profit)
                   * (trainingResult.OrderCount + validationResult.OrderCount)
                   * (trainingResult.PlusCount - trainingResult.MinusCount + validationResult.PlusCount - validationResult.MinusCount)
                   * buysell;

        }

        private FxTesterResult GetProfit(GeneticNeuronDLL1 dll, TradingItem item, LearningData learningData)
        {
            var tester = new DirectionTester(dll, learningData);
            FxTesterResult result = tester.Run();
            return result;
        }

        private GeneticNeuronDLL1 GetNeuronDll(TradingItem item)
        {
            if (_dll == null)
                _dll = CreateNeuronDll();
            _dll.SetAlpha(item.Alpha);
            SetWeigths(_dll, item);
            return _dll;
        }

        private void SetWeigths(GeneticNeuronDLL1 dll, TradingItem item)
        {
            var weights = dll.GetWeigths();
            var values = item.Values;
            var offset = TradingItem.OFFSET_VALUES;
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = values[offset] * 100.0F - 50.0F;
                //weights[i] = values[offset] * 2.0 - 1.0;
                ++offset;
            }
        }
    }
}
