using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Messages;
using MasterDataFlow.Network;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Genetic;
using MasterDataFlow.Trading.Indicators;
using MasterDataFlow.Trading.Tester;
using MasterDataFlow.Trading.Ui.Business.Data;
using MasterDataFlow.Trading.Ui.Business.IO;
using Microsoft.CodeAnalysis.CSharp;
using Trady.Analysis;
using Trady.Analysis.Extension;
using Trady.Core.Infrastructure;
using Trady.Importer.Csv;
using DirectionTester = MasterDataFlow.Trading.Genetic.DirectionTester;

namespace MasterDataFlow.Trading.Ui.Business.Teacher
{
    public class DisplayChartPricesArgs
    {
        public DisplayChartPricesArgs()
        {
        }
    }

    public delegate void DisplayChartPricesHandler(object sender, DisplayChartPricesArgs args);


    public class IterationEndArgs
    {
        public IterationEndArgs(int iteration, long elapsedMilliseconds)
        {
            Iteration = iteration;
            ElapsedMilliseconds = elapsedMilliseconds;
        }

        public int Iteration { get; private set; }
        public long ElapsedMilliseconds { get; private set; }
    }

    public delegate void IterationEndHandler(object sender, IterationEndArgs args);



    public class DisplayBestArgs
    {
        public DisplayBestArgs(TradingItem neuronItem)
        {
            NeuronItem = neuronItem;
        }

        public TradingItem NeuronItem { get; private set; }
    }

    public delegate void DisplayBestHandler(object sender, DisplayBestArgs args);

    public class PeriodChangedArgs
    {
        public PeriodChangedArgs(DateTime startTraining, DateTime startValidation, DateTime startTest)
        {
            StartTraining = startTraining;
            StartValidation = startValidation;
            StartTest = startTest;
        }

        public DateTime StartTraining { get; private set; }
        public DateTime StartValidation { get; private set; }
        public DateTime StartTest { get; private set; }

    }

    public delegate void PeriodChangedHandler(object sender, PeriodChangedArgs args);

    public class BaseCommandController
    {
        private readonly int _processorCount;

        public BaseCommandController(int processorCount)
        {
            _processorCount = processorCount;
        }

        public async Task Execute()
        {
            using (var remote = new RemoteEnvironment())
            {
                await ExecuteCommand(remote.CommandWorkflow);
            }
        }

        private TradingDataObject _dataObject = null;
        private int _iteration;
        private IEnumerable<IOhlcv> _candles = null;
        private Bar[] _tradingBars = null;
        private int[] _zigZag = null;
        private ZigZagValue[] _zigZagLearningData = null;
        private readonly InputDataCollection _inputData = new InputDataCollection();
        private LearningData _trainingData = null;
        private LearningData _validationData = null;
        private LearningData _testData = null;

        private readonly Reader _reader = new Reader();
        private readonly Writer _writer = new Writer();

        private const int IndicatorsOffset = 50;

        #region Properties
        public int PopulationFactor { get; set; } = 1;

        public TradingDataObject DataObject
        {
            get { return _dataObject; }
        }

        public LearningData TestData
        {
            get { return _testData; }
        }

        public Bar[] TradingBars
        {
            get { return _tradingBars; }
        }

        public int[] ZigZag
        {
            get { return _zigZag; }
        }

        #endregion

        #region Events
        public event PeriodChangedHandler SetPeriodsEvent;
        public event DisplayBestHandler DisplayBestEvent;
        public event IterationEndHandler IterationEndEvent;
        public event DisplayChartPricesHandler DisplayChartPricesEvent;

        #endregion
        private async Task ExecuteCommand(CommandWorkflow commandWorkflow)
        {
            await Task.Factory.StartNew(async () => {

                using (var @event = new ManualResetEvent(false))
                {
                    try
                    {

                        _dataObject = await CreateDataObject();

                        DisplayChartPrices();

                        var tradingItem = _reader.ReadItem(_dataObject.ItemInitData);
                        if (tradingItem != null)
                        {
                            DisplayBest(tradingItem);
                            _dataObject.InitPopulation = new List<float[]>();
                            _dataObject.InitPopulation.Add(tradingItem.Values);
                        }

                        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

                        commandWorkflow.MessageRecieved +=
                            (key, message) => {
                                if (message is StopCommandMessage stopMessage)
                                {
                                    var best = (TradingItem)(stopMessage.Data as GeneticInfoDataObject).Best;
                                    DisplayBest(best);
                                    @event.Set();
                                }

                                if (message is GeneticEndCycleMessage endCycleMessage)
                                {
                                    ++_iteration;

                                    sw.Stop();
                                    IterationEnd(_iteration, sw.ElapsedMilliseconds);
                                    sw = System.Diagnostics.Stopwatch.StartNew();

                                    var best = (TradingItem)endCycleMessage.Data.Best;
                                    DisplayBest(best);

                                }
                            };

                        commandWorkflow.Start<TradingCommand>(_dataObject);
                        @event.WaitOne(1000000);
                    }
                    catch (Exception e)
                    {
                        throw;
                    }

                }

            });
        }

        private async Task<TradingDataObject> CreateDataObject()
        {
            var itemInitData = _reader.ReadItemInitData();

            await LoadInputData(itemInitData);

            var result = new TradingDataObject(itemInitData, _trainingData, _validationData,
                100 * PopulationFactor, 33 * PopulationFactor, _processorCount);

            return result;
        }


        private async Task LoadInputData(TradingItemInitData itemInitData)
        {
            var csvImporter = new CsvImporter(@"Data\AFLT.csv", new CultureInfo("en-US"));
            //var csvImporter = new CsvImporter(@"Data\SBER.csv", new CultureInfo("en-US"));
            _candles = await csvImporter.ImportAsync("fb");

            _tradingBars = Helper.CandlesToBars(_candles);

            CalculateZigZag();

            SetDataBoundaris(itemInitData);
        }

        private void SetDataBoundaris(TradingItemInitData itemInitData)
        {
            var startTrainingDate = _tradingBars[IndicatorsOffset + itemInitData.HistoryWidowLength].Time.Date.AddDays(1);

            var endTestDate = _tradingBars[_tradingBars.Length - 1].Time.Date;
            var days = (endTestDate - startTrainingDate).TotalDays;

            var trainingDays = days * 60 / 100;
            //var validationDays = days * 35 / 100;
            //var testDays = days * 5 / 100;

            var validationDays = days * 20 / 100;
            var testDays = days * 20 / 100;


            var startValidationDate = startTrainingDate.AddDays(trainingDays);
            var startTestDate = startValidationDate.AddDays(validationDays);

            var inputValues = new List<InputValues>();
            var inputs = _inputData.GetInputs();
            foreach (var input in inputs)
            {
                var values = input.GetValues(_tradingBars);
                inputValues.Add(values);
                if (itemInitData.InputData.Indicators.IsNormalizeValues)
                    input.Normalize(values);

            }

            _trainingData = CreateLearningData(itemInitData, inputValues, startTrainingDate, trainingDays);
            _validationData = CreateLearningData(itemInitData, inputValues, startValidationDate, validationDays);
            _testData = CreateLearningData(itemInitData, inputValues, startTestDate, testDays);

            SetPeriodsEvent?.Invoke(this, new PeriodChangedArgs(startTrainingDate, startValidationDate, startTestDate));
        }

        private LearningData CreateLearningData(TradingItemInitData itemInitData, List<InputValues> inputValues, DateTime startDate, double days)
        {
            var result = new LearningData();
            result.Prices = _tradingBars
                .SkipWhile(t => t.Time < startDate)
                .TakeWhile(t => t.Time < startDate.AddDays(days)).ToArray();

            var indicators = new List<LearningInputData>();


            var firstTime = result.Prices[0].Time;
            var lastTime = result.Prices[result.Prices.Length - 1].Time;

            var initTimes = inputValues[0].Values.Select(t => t.Time).ToArray();
            var firstIndicatorIndex = Array.IndexOf(initTimes, firstTime) - itemInitData.HistoryWidowLength - 1;
            var lastIndicatorIndex = Array.IndexOf(initTimes, lastTime) - 1;

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
            _zigZag = ZigZagIndicator.Calculate(_tradingBars, 0, _tradingBars.Length - 1, 3.5m).ToArray();
            _zigZagLearningData = _tradingBars.Select(t => new ZigZagValue { Time = t.Time, Value = Int32.MinValue }).ToArray();

            var isHigh = _tradingBars[_zigZag[0]].High > _tradingBars[_zigZag[1]].Low;

            for (int i = 1; i < _zigZag.Length; i++)
            {
                int jBegin;
                if (i != 1)
                {
                    jBegin = _zigZag[i - 1] + 1;
                    _zigZagLearningData[_zigZag[i]].Value = 0;
                }
                else
                {
                    jBegin = _zigZag[i - 1];
                }
                if (isHigh)
                {
                    for (int j = jBegin; j < _zigZag[i]; j++)
                    {
                        _zigZagLearningData[j].Value = -1;
                    }
                }
                else
                {
                    for (int j = jBegin; j < _zigZag[i]; j++)
                    {
                        _zigZagLearningData[j].Value = 1;
                    }
                }
                isHigh = !isHigh;
            }
        }

        // external 

        private void DisplayChartPrices()
        {
            DisplayChartPricesEvent?.Invoke(this, new DisplayChartPricesArgs());
        }

        private void IterationEnd(int iteration, long elapsedMilliseconds)
        {
            IterationEndEvent?.Invoke(this, new IterationEndArgs(iteration, elapsedMilliseconds));
        }

        private void DisplayBest(TradingItem neuronItem, bool isSaveBest = true)
        {
            DisplayBestEvent?.Invoke(this, new DisplayBestArgs(neuronItem));

            var dll = NeuronNetwork.CreateNeuronDll(_dataObject.ItemInitData.NeuronNetwork, neuronItem);
            var tester = new DirectionTester(dll, neuronItem, _testData);
            var testResult = tester.Run();

            if (isSaveBest)
                _writer.SaveBest(neuronItem, testResult);

        }
    }
}
