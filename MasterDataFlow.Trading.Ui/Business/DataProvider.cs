using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Genetic;
using MasterDataFlow.Trading.Indicators;
using MasterDataFlow.Trading.IO;
using MasterDataFlow.Trading.Shared.Data;
using MasterDataFlow.Trading.Ui.Business.Teacher;
using Trady.Core.Infrastructure;
using Trady.Importer.Csv;

namespace MasterDataFlow.Trading.Ui.Business
{
    public class DisplayChartPricesArgs
    {
        public DisplayChartPricesArgs()
        {
        }
    }

    public delegate void DisplayChartPricesHandler(object sender, DisplayChartPricesArgs args);

    public class PeriodChangedArgs
    {
        public PeriodChangedArgs(LearningData[] trainings, LearningData test)
        {
            Trainings = trainings;
            Test = test;
        }

        public LearningData[] Trainings { get; private set; }
        public LearningData Test { get; private set; }

    }

    public delegate void PeriodChangedHandler(object sender, PeriodChangedArgs args);


    public class DataProvider
    {
        private readonly string _stockName;
        private readonly Reader _reader;
        private IEnumerable<IOhlcv> _candles = null;
        private ZigZagValue[] _zigZagLearningData = null;
        private readonly InputDataCollection _inputData = new InputDataCollection();

        public DataProvider(string stockName)
        {
            _stockName = stockName;
            _reader = new Reader(new InputDataCollection());
            Trainings = new List<LearningData>();
        }

        public async Task LoadData(string filename = "genetic.save")
        {
            ItemInitData = _reader.ReadItemInitData(filename);
            await LoadInputData();
        }

        public TradingItem ReadTradingItem(string filename = "genetic.save")
        {
            var tradingItem = _reader.ReadItem(ItemInitData, filename);
            return tradingItem;
        } 

        public TradingItemInitData ItemInitData { get; private set; }
        public Bar[] TradingBars { get; private set; }
        public int[] ZigZag { get; private set; }

        public List<LearningData> Trainings { get; private set; }

        public LearningData TestData { get; private set; }

        #region Events

        public event DisplayChartPricesHandler DisplayChartPricesEvent;

        private void DisplayChartPrices()
        {
            DisplayChartPricesEvent?.Invoke(this, new DisplayChartPricesArgs());
        }

        public event PeriodChangedHandler SetPeriodsEvent;

        #endregion

        private async Task LoadInputData()
        {
            var csvImporter = new CsvImporter($"Data\\{_stockName}.csv", new CultureInfo("en-US"));
            //var csvImporter = new CsvImporter(@"Data\SBER.csv", new CultureInfo("en-US"));
            _candles = await csvImporter.ImportAsync("fb");

            TradingBars = Helper.CandlesToBars(_candles);

            CalculateZigZag();

            SetDataBoundaris();

            DisplayChartPrices();
        }


        private void SetDataBoundaris()
        {
            // Получаем первый день данных Смещение получается за счёт окна для вычисления данных Constants.IndicatorsOffset
            // и HistoryWidowLength
            var startTrainingDate = TradingBars[Constants.IndicatorsOffset + ItemInitData.HistoryWidowLength].Time.Date.AddDays(1);

            var endTestDate = TradingBars[TradingBars.Length - 1].Time.Date;
            var days = (endTestDate - startTrainingDate).TotalDays;

            var testDays = 14;
            var trainingDays = days - testDays;
            var foldsCount = 5;
            var foldsDays = (int)(trainingDays / foldsCount);
            trainingDays = foldsDays * foldsCount;
            var startTestDate = endTestDate.AddDays(-testDays);
            startTrainingDate = startTestDate.AddDays(-trainingDays);

            var inputValues = new List<InputValues>();
            var inputs = _inputData.GetInputs();
            foreach (var input in inputs)
            {
                var values = input.GetValues(TradingBars);
                inputValues.Add(values);
                if (ItemInitData.InputData.Indicators.IsNormalizeValues)
                {
                    var scaler = new MinMaxScaler(input.GetMin(), input.GetMax());
                    scaler.Transform(values);
                }

            }

            for (int i = 0; i < foldsCount; i++)
            {
                var startFoldDate = startTrainingDate.AddDays(i * foldsDays);
                var trainingData = CreateLearningData(inputValues, startFoldDate, foldsDays);
                Trainings.Add(trainingData);
            }

            // тест дата
            TestData = CreateLearningData(inputValues, startTestDate, testDays);

            SetPeriodsEvent?.Invoke(this, new PeriodChangedArgs(Trainings.ToArray(), TestData));
        }

        private LearningData CreateLearningData(List<InputValues> inputValues, DateTime startDate, double days)
        {
            var result = new LearningData();
            result.StartDateTime = startDate;
            // Получаем список цен в требуем диапазоне
            result.Prices = TradingBars
                .SkipWhile(t => t.Time < startDate)
                .TakeWhile(t => t.Time < startDate.AddDays(days)).ToArray();

            var indicators = new List<LearningInputData>();


            var firstTime = result.Prices[0].Time;
            var lastTime = result.Prices[result.Prices.Length - 1].Time;

            var initTimes = inputValues[0].Values.Select(t => t.Time).ToArray();
            //var firstIndicatorIndex = Array.IndexOf(initTimes, firstTime) - itemInitData.HistoryWidowLength - 1;
            //var lastIndicatorIndex = Array.IndexOf(initTimes, lastTime) - 1;
            var firstIndicatorIndex = Array.IndexOf(initTimes, firstTime) - ItemInitData.HistoryWidowLength;
            var lastIndicatorIndex = Array.IndexOf(initTimes, lastTime);

            foreach (var inputValue in inputValues)
            {
                var learningIndicator = new LearningInputData
                {
                    Name = inputValue.Name,
                    Values = new float[lastIndicatorIndex - firstIndicatorIndex + 1],
                    Times = new DateTime[lastIndicatorIndex - firstIndicatorIndex + 1],
                };
                var values = inputValue.Values.Select(t => t.Value).ToArray();
                var times = inputValue.Values.Select(t => t.Time).ToArray();
                Array.Copy(values, firstIndicatorIndex, learningIndicator.Values, 0, learningIndicator.Values.Length);
                Array.Copy(times, firstIndicatorIndex, learningIndicator.Times, 0, learningIndicator.Times.Length);
                indicators.Add(learningIndicator);
            }
            result.Indicators = indicators.ToArray();

            result.ZigZags = _zigZagLearningData
                .SkipWhile(t => t.Time < startDate)
                .TakeWhile(t => t.Time < startDate.AddDays(days)).ToArray();

            return result;
        }

        private void CalculateZigZag()
        {
            ZigZag = ZigZagIndicator.Calculate(TradingBars, 0, TradingBars.Length - 1, 3.5m).ToArray();
            _zigZagLearningData = TradingBars.Select(t => new ZigZagValue { Time = t.Time, Value = Int32.MinValue }).ToArray();

            var isHigh = TradingBars[ZigZag[0]].High > TradingBars[ZigZag[1]].Low;

            for (int i = 1; i < ZigZag.Length; i++)
            {
                int jBegin;
                if (i != 1)
                {
                    jBegin = ZigZag[i - 1] + 1;
                    _zigZagLearningData[ZigZag[i]].Value = 0;
                }
                else
                {
                    jBegin = ZigZag[i - 1];
                }
                if (isHigh)
                {
                    for (int j = jBegin; j < ZigZag[i]; j++)
                    {
                        _zigZagLearningData[j].Value = -1;
                    }
                }
                else
                {
                    for (int j = jBegin; j < ZigZag[i]; j++)
                    {
                        _zigZagLearningData[j].Value = 1;
                    }
                }
                isHigh = !isHigh;
            }
        }
    }
}
