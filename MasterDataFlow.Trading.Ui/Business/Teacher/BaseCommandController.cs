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
        private List<LearningDataIndicator> _indicators = null;
        private LearningData _trainingData = null;
        private LearningData _validationData = null;
        private LearningData _testData = null;

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

                        var tradingItem = LoadItem(_dataObject.ItemInitData, _dataObject.TrainingData);
                        if (tradingItem != null)
                        {
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
            var itemInitData = LoadItemInitData();

            await LoadInputData(itemInitData);

            var result = new TradingDataObject(itemInitData, _trainingData, _validationData,
                100 * PopulationFactor, 33 * PopulationFactor, _processorCount);

            return result;
        }


        private async Task LoadInputData(TradingItemInitData itemInitData)
        {
            //var csvImporter = new CsvImporter(@"Data\AFLT.csv", new CultureInfo("en-US"));
            var csvImporter = new CsvImporter(@"Data\SBER.csv", new CultureInfo("en-US"));
            _candles = await csvImporter.ImportAsync("fb");

            _tradingBars = Helper.CandlesToBars(_candles);

            CreateIndicatorsValues();

            CalculateZigZag();

            SetDataBoundaris(itemInitData);
        }



        private void CreateIndicatorsValues()
        {
            _indicators = new List<LearningDataIndicator>();

            // RSI
            AddIndicator("RSI 3", _candles.Rsi(3));
            AddIndicator("RSI 7", _candles.Rsi(7));
            AddIndicator("RSI 14", _candles.Rsi(14));
            AddIndicator("RSI 21", _candles.Rsi(21));
            AddIndicator("RSI 28", _candles.Rsi(28));

            // Parabolic SAR
            var psar = _candles.Sar(0.02M, 0.2M);
            AddIndicator("SAR", psar);

            // EMA
            AddIndicator("EMA 3", _candles.Ema(3));
            AddIndicator("EMA 5", _candles.Ema(5));
            AddIndicator("EMA 10", _candles.Ema(10));
            AddIndicator("EMA 15", _candles.Ema(15));
            AddIndicator("EMA 20", _candles.Ema(20));
            AddIndicator("EMA 25", _candles.Ema(25));

            // MACD
            var macds = _candles.Macd(12, 26, 9);

            AddIndicator("MACD Histogram", macds.Select(t => (float)t.Tick.MacdHistogram).ToArray(), macds.Select(t => t.DateTime.Value.DateTime).ToArray());
            AddIndicator("MACD SignalLine", macds.Select(t => (float)t.Tick.SignalLine).ToArray(), macds.Select(t => t.DateTime.Value.DateTime).ToArray());
            AddIndicator("MACD Line", macds.Select(t => (float)t.Tick.MacdLine).ToArray(), macds.Select(t => t.DateTime.Value.DateTime).ToArray());

            AddIndicator("Volumes", _tradingBars.Select(t => (float)t.Volume).ToArray(),
                _tradingBars.Select(t => t.Time).ToArray());
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

        // https://www.youtube.com/watch?v=IGKSaH4yz-g

        private void SetDataBoundaris(TradingItemInitData itemInitData)
        {
            var startTrainingDate = _tradingBars[IndicatorsOffset + itemInitData.HistoryWidowLength].Time.Date.AddDays(1);

            var endTestDate = _tradingBars[_tradingBars.Length - 1].Time.Date;
            var days = (endTestDate - startTrainingDate).TotalDays;

            var trainingDays = days * 60 / 100;
            var validationDays = days * 35 / 100;
            var testDays = days * 5 / 100;

            var startValidationDate = startTrainingDate.AddDays(trainingDays);
            var startTestDate = startValidationDate.AddDays(validationDays);

            //var startTestDate = new DateTime(2017, 12, 9);

            _trainingData = CreateLearningData(itemInitData, startTrainingDate, trainingDays);
            _validationData = CreateLearningData(itemInitData, startValidationDate, validationDays);
            _testData = CreateLearningData(itemInitData, startTestDate, testDays);

            SetPeriodsEvent?.Invoke(this, new PeriodChangedArgs(startTrainingDate, startValidationDate, startTestDate));
        }

        private LearningData CreateLearningData(TradingItemInitData itemInitData, DateTime startDate, double days)
        {
            var result = new LearningData();
            result.Prices = _tradingBars
                .SkipWhile(t => t.Time < startDate)
                .TakeWhile(t => t.Time < startDate.AddDays(days)).ToArray();

            var indicators = new List<LearningDataIndicator>();


            var firstTime = result.Prices[0].Time;
            var lastTime = result.Prices[result.Prices.Length - 1].Time;

            var firstIndicatorIndex = Array.IndexOf(_indicators[0].Times, firstTime) - itemInitData.HistoryWidowLength - 1;
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
                if (itemInitData.InputData.Indicators.IsNormalizeValues)
                    learningIndicator.Normalize();
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

        #region Read Write Region 

        private Guid lastGuid;

        private void SaveBest(TradingItem item, TesterResult futureResult)
        {
            lock (this)
            {

                if (item.TrainingTesterResult == null || item.ValidationTesterResult == null)
                    return;

                if (lastGuid != null && lastGuid.ToString() == item.Guid.ToString())
                    return;


                using (StreamWriter writer = new StreamWriter("genetic.save"))
                {
                    WriteNew(writer, _indicators, item);
                }

                //if (!(
                //    item.TrainingTesterResult.Profit > 0 && item.ValidationTesterResult.Profit > 0
                //    && futureResult.Profit > 0)
                //) return;


                lastGuid = item.Guid;

                if (!Directory.Exists("Best"))
                {
                    Directory.CreateDirectory("Best");
                }
                string fileName = "Best/best.csv";

                bool isNoFile = !File.Exists(fileName);

                using (StreamWriter writer = new StreamWriter(fileName, true))
                {
                    if (isNoFile)
                        AddHeader(writer);
                    AddItemToFile(writer, item, futureResult);
                }
                SaveBestItem(item);
            }
        }

        private void AddHeader(StreamWriter writer)
        {
            string line =
                "Date, ";
            line += "Guid, ";
            line += "Fitness, ";
            line += "StopLoss, ";
            line += AddHeaderTestResult("tr");
            line += AddHeaderTestResult("pr");
            line += AddHeaderTestResult("fr");
            writer.WriteLine(line);
        }

        private string AddHeaderTestResult(string prefix)
        {
            string line = "";
            line += prefix + ".Profit, ";
            line += prefix + ".OrderCount, ";
            line += prefix + ".PlusCount, ";
            line += prefix + ".MinusCount, ";
            line += prefix + ".MaxEquity, ";
            line += prefix + ".MinEquity, ";
            line += prefix + ".PlusEquityCount, ";
            line += prefix + ".MinusEquityCount, ";
            return line;
        }

        private void AddItemToFile(StreamWriter writer, TradingItem item, TesterResult futureResult)
        {
            string line = DateTime.Now.ToString(CultureInfo.InvariantCulture) + ", ";
            line += item.Guid.ToString() + ", ";
            line += item.Fitness.ToString("F10") + ", ";
            line += item.StopLoss.ToString("F10") + ", ";
            line = SaveTestResult(item.TrainingTesterResult, line);
            line = SaveTestResult(item.ValidationTesterResult, line);
            line = SaveTestResult(futureResult, line);

            writer.WriteLine(line);
        }

        private string SaveTestResult(TesterResult tr, string line)
        {
            line += tr.Profit.ToString("F10") + ", ";
            line += tr.OrderCount.ToString("D") + ", ";
            line += tr.PlusCount.ToString("D") + ", ";
            line += tr.MinusCount.ToString("D") + ", ";
            line += tr.MaxEquity.ToString("F10") + ", ";
            line += tr.MinEquity.ToString("F10") + ", ";
            line += tr.PlusEquityCount.ToString("D") + ", ";
            line += tr.MinusEquityCount.ToString("D") + ", ";
            return line;
        }

        private void SaveBestItem(TradingItem item)
        {
            StreamWriter writer = new StreamWriter("Best/" + item.Guid.ToString() + ".save");
            try
            {
                WriteNew(writer, _indicators, item);
            }
            finally
            {
                writer.Close();
            }
        }

        private void WriteNew(TextWriter writer, List<LearningDataIndicator> indicators, TradingItem item)
        {
            XElement root = new XElement("genetic");

            item.InitData.Write(root);

            WriteItem(root, indicators, item);
            root.Save(writer);
        }


        private void WriteItem(XElement root, List<LearningDataIndicator> indicators, TradingItem item)
        {
            var itemElement = new XElement("item");
            root.Add(itemElement);

            itemElement.Add(new XElement("fitness", item.Fitness.ToString(CultureInfo.InvariantCulture)));
            itemElement.Add(new XElement("guid", item.Guid.ToString()));
            itemElement.Add(new XElement("historyWidowLength", item.InitData.HistoryWidowLength.ToString(CultureInfo.InvariantCulture)));


            itemElement.Add(new XElement("valuesCount", item.Values.Length.ToString(CultureInfo.InvariantCulture)));

            var namedValues = new XElement("namedValues");
            itemElement.Add(namedValues);

            for (var i = 0; i < item.InitData.InputData.Indicators.IndicatorNumber; i++)
            {
                var value = item.Values[i];
                var v = new XElement("v", value.ToString(CultureInfo.InvariantCulture));
                v.Add(new XAttribute("type", "indicator"));
                v.Add(new XAttribute("name", indicators[(int)value].Name));
                namedValues.Add(v);
            }

            var alfa = new XElement("v", item.Values[item.InitData.OFFSET_ALPHA].ToString(CultureInfo.InvariantCulture));
            alfa.Add(new XAttribute("type", "alfa"));
            namedValues.Add(alfa);

            var stopLoss = new XElement("v", item.Values[item.InitData.OFFSET_STOPLOSS].ToString(CultureInfo.InvariantCulture));
            stopLoss.Add(new XAttribute("type", "stopLoss"));
            namedValues.Add(stopLoss);

            var values = new XElement("values");
            itemElement.Add(values);
            for (var i = item.InitData.OFFSET_VALUES; i < item.Values.Length; i++)
            {
                var value = item.Values[i];
                values.Add(new XElement("v", value.ToString(CultureInfo.InvariantCulture)));
            }
        }

        private TradingItem LoadItem(TradingItemInitData itemInitData, LearningData learningData)
        {
            if (!File.Exists("genetic.save"))
                return null;

            StreamReader reader = new StreamReader("genetic.save");
            try
            {

                XElement root = XElement.Load(reader, LoadOptions.None);
                var item = CreateItem(root, itemInitData);
                ReadItemValues(root, item, learningData);

                DisplayBest(item);
                return item;
            }
            finally
            {
                reader.Close();
            }
        }

        private TradingItemInitData LoadItemInitData()
        {
            if (!File.Exists("genetic.save"))
            {
                return new TradingItemInitData();
            }

            StreamReader reader = new StreamReader("genetic.save");
            try
            {
                XElement root = XElement.Load(reader, LoadOptions.None);
                var result = new TradingItemInitData();
                result.Read(root);
                return result;
            }
            finally
            {
                reader.Close();
            }
        }


        private void ReadItemValues(XElement root, TradingItem item, LearningData learningData)
        {
            XElement itemElement = root.Element("item");

            XElement eFitness = itemElement.Element("fitness");
            item.Fitness = Double.Parse(eFitness.Value);

            XElement guid = itemElement.Element("guid");
            item.Guid = Guid.Parse(guid.Value);

            XElement eValues = itemElement.Element("namedValues");
            int offset = 0;
            foreach (XElement eValue in eValues.Elements("v"))
            {
                var attributes = eValue.Attributes().ToArray();
                if (attributes.Any(t => t.Name.LocalName == "type" && t.Value == "indicator"))
                {
                    var name = attributes.Single(t => t.Name.LocalName == "name");
                    item.Values[offset] = GetIndicatorIndex(learningData, name.Value);
                }
                else
                {
                    item.Values[offset] = ParseFloat(eValue.Value);
                }
                ++offset;
            }

            eValues = itemElement.Element("values");
            foreach (XElement eValue in eValues.Elements("v"))
            {
                item.Values[offset] = ParseFloat(eValue.Value);
                ++offset;
            }

        }

        private float GetIndicatorIndex(LearningData learningData, string name)
        {
            var search = new LearningDataIndicator.IndicatorSearch(name);
            var index = learningData.Indicators.ToList().FindIndex(search.Match);
            if (index < 0)
                throw new Exception("Invalid indicator name:" + name);
            return index;
        }

        private float ParseFloat(string value)
        {
            var result = Convert.ToDouble(value);
            return (float)result;
        }

        private TradingItem CreateItem(XElement root, TradingItemInitData itemInitData)
        {
            TradingItem item = new TradingItem(itemInitData);
            return item;
        }


        #endregion 

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
                SaveBest(neuronItem, testResult);

        }
    }
}
