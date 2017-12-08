using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MasterDataFlow.Common.Tests;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Messages;
using MasterDataFlow.Network;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Genetic;
using Trady.Analysis;
using Trady.Analysis.Extension;
using Trady.Core;
using Trady.Core.Infrastructure;
using Trady.Importer;
using Trady.Importer.Csv;

namespace MasterDataFlow.Trading.Ui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            using (var remote = new RemoteEnvironment())
            {
                await Execute(remote.CommandWorkflow);
            }

        }

        public async Task Execute(CommandWorkflow commandWorkflow)
        {
            using (var @event = new ManualResetEvent(false))
            {
                var dataObject = await CreateDataObject();
                var instancesCount = 10;

                commandWorkflow.MessageRecieved += 
                    (key, message) => {
                        var stopMessage = message as StopCommandMessage;
                        if (stopMessage != null)
                        {
                            var best = (TradingItem)(stopMessage.Data as GeneticStopDataObject).Best;
                            @event.Set();
                        }
                    };

               commandWorkflow.Start<TradingCommand>(dataObject);
                @event.WaitOne(1000000);

                //var theBests = new List<VariableCrossoverGeneticItem>();
                //var completedInstances = 0;
                //commandWorkflow.MessageRecieved += (key, message) =>
                //{
                //    Console.WriteLine("Response from {0}", key);
                //    var stopMessage = message as StopCommandMessage;
                //    if (stopMessage != null)
                //    {
                //        var best = (VariableCrossoverGeneticItem)(stopMessage.Data as GeneticStopDataObject).Best;
                //        lock (this)
                //        {
                //            theBests.Add(best);
                //            ++completedInstances;
                //            if (completedInstances == instancesCount)
                //            {
                //                @event.Set();
                //            }
                //        }
                //    }
                //};

                //for (int i = 0; i < instancesCount; i++)
                //{
                //    commandWorkflow.Start<VariableCrossoverGeneticCommand>(dataObject);
                //}
                //@event.WaitOne(1000000);
                //Console.WriteLine("The bests");
                //foreach (var theBestItem in theBests)
                //{
                //    PrintTheBest(theBestItem);
                //}
                //Console.WriteLine("The best fitness is {0}", theBests.Max(t => t.Fitness));

                //dataObject = CreateTravelingSalesmanProblemInitData();
                ////dataObject.RepeatCount = 200;
                //dataObject.InitPopulation = new List<int[]>();
                //dataObject.InitLengths = new List<int[]>();
                //for (int i = 0; i < theBests.Count; i++)
                //{
                //    dataObject.InitPopulation.Add(theBests[i].Values);
                //    dataObject.InitLengths.Add(theBests[i].Lengths);
                //}
                //@event.Reset();
                //theBests.Clear();
                //instancesCount = 1;
                //completedInstances = 0;
                //commandWorkflow.Start<VariableCrossoverGeneticCommand>(dataObject);
                //@event.WaitOne(1000000);
                //Console.WriteLine("The bests");
                //foreach (var theBestItem in theBests)
                //{
                //    PrintTheBest(theBestItem);
                //}
            }

        }

        private Bar[] CandlesToBars(IEnumerable<IOhlcv> candles)
        {
            var result = candles.Select(t => new Bar
            {
                Close = (double)t.Close,
                High = (double)t.High,
                Low = (double)t.Low,
                Open = (double)t.Open,
                Volume = (double)t.Volume,
                Time = t.DateTime.DateTime,
            }).ToArray();
            return result;
        }

        private const int HistoryWindowLength = 48;
        private const int IndicatorsOffset = 50;

        private async Task<TradingDataObject> CreateDataObject()
        {
            await LoadInputData();


            var result = new TradingDataObject(_trainingData, _validationData, HistoryWindowLength,
                100 * (int)nudPopulationFactor.Value, 33 * (int)nudPopulationFactor.Value);



            return result;
        }

        private IEnumerable<IOhlcv> _candles = null;
        private Bar[] _tradingBars = null;
        private List<LearningDataIndicator> _indicators = null;
        private LearningData _trainingData = null;
        private LearningData _validationData = null;
        private LearningData _testData = null;

        private async Task LoadInputData()
        {
            var csvImporter = new CsvImporter(@"Data\output.csv", new CultureInfo("en-US"));
            _candles = await csvImporter.ImportAsync("fb");

            _tradingBars = CandlesToBars(_candles);

            CreateIndicatorsValues();

            SetDataBoundaris();
        }

        private void CreateIndicatorsValues()
        {
            _indicators = new List<LearningDataIndicator>();

            // MACD
            var macds = _candles.Macd(12, 26, 9);
            AddIndicator("MACD Histogram", macds.Select(t => (float) t.Tick.MacdHistogram).ToArray(), macds.Select(t => t.DateTime.Value.DateTime).ToArray());
            AddIndicator("MACD SignalLine", macds.Select(t => (float)t.Tick.SignalLine).ToArray(), macds.Select(t => t.DateTime.Value.DateTime).ToArray());
            AddIndicator("MACD Line", macds.Select(t => (float)t.Tick.MacdLine).ToArray(), macds.Select(t => t.DateTime.Value.DateTime).ToArray());

            // RSI
            IReadOnlyList<AnalyzableTick<decimal?>> rsi = _candles.Rsi(14);
            AddIndicator("RSI ", rsi);

            // Parabolic SAR
            var psar = _candles.Sar(0.02M, 0.2M);
            AddIndicator("SAR ", psar);

            // EMA
            var ema5 = _candles.Ema(5);
            AddIndicator("EMA 5", ema5);
            var ema10 = _candles.Ema(10);
            AddIndicator("EMA 10", ema10);
            var ema15 = _candles.Ema(15);
            AddIndicator("EMA 15", ema15);
            var ema20 = _candles.Ema(20);
            AddIndicator("EMA 20", ema20);
        }

        private void AddIndicator(string name, float[] values, DateTime[] times)
        {
            var indicator = new LearningDataIndicator
                            {
                                Name = name,
                                Values = values,
                                Times = times,
                            };
            _indicators.Add(indicator);
        }

        private void AddIndicator(string name, IReadOnlyList<AnalyzableTick<decimal?>> ticks)
        {
            var indicator = new LearningDataIndicator
                            {
                                Name = name,
                                Values = ticks.Select(t => t.Tick.HasValue ? (float)t.Tick.Value : 0F).ToArray(),
                                Times = ticks.Select(t => t.DateTime.HasValue ? t.DateTime.Value.DateTime : new DateTime()).ToArray(),
                            };
            _indicators.Add(indicator);
        }



        private void SetDataBoundaris()
        {
            var startTrainingDate = _tradingBars[IndicatorsOffset + HistoryWindowLength].Time.Date.AddDays(1);
            var endTestDate = _tradingBars[_tradingBars.Length - 1].Time.Date;
            var days = (endTestDate - startTrainingDate).TotalDays;

            var trainingDays = days * 60 / 100;
            var validationDays = days * 20 / 100;
            var testDays = days * 20 / 100;

            var startValidationDate = startTrainingDate.AddDays(trainingDays);
            var startTestDate = startValidationDate.AddDays(validationDays);

            _trainingData = CreateLearningData(startTrainingDate, trainingDays);
            _validationData = CreateLearningData(startValidationDate, validationDays);
            _testData = CreateLearningData(startTestDate, testDays);
        }

        private LearningData CreateLearningData(DateTime startDate, double days)
        {
            var result = new LearningData();
            result.Prices = _tradingBars
                .SkipWhile(t => t.Time < startDate)
                .TakeWhile(t => t.Time < startDate.AddDays(days)).ToArray();

            var indicators = new List<LearningDataIndicator>();


            var firstTime = result.Prices[0].Time;
            var lastTime = result.Prices[result.Prices.Length - 1].Time;

            var firstIndicatorIndex = Array.IndexOf(_indicators[0].Times, firstTime) - HistoryWindowLength - 1;
            var lastIndicatorIndex = Array.IndexOf(_indicators[0].Times, lastTime) - 1;

            foreach (var indicator in _indicators)
            {
                var learningIndicator = new LearningDataIndicator
                {
                    Name = indicator.Name,
                    Values = new float[lastIndicatorIndex - firstIndicatorIndex + 1],
                    Times = new DateTime[lastIndicatorIndex - firstIndicatorIndex + 1],
                };

                Array.Copy(indicator.Values, firstIndicatorIndex, learningIndicator.Values, 0, learningIndicator.Values.Length);
                Array.Copy(indicator.Times, firstIndicatorIndex, learningIndicator.Times, 0, learningIndicator.Times.Length);
                indicators.Add(learningIndicator);
            }

            result.Indicators = indicators.ToArray();
            return result;
        }
    }
}
