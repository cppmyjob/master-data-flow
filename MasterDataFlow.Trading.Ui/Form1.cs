using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;
using System.Xml.XPath;
using MasterDataFlow.Common.Tests;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Messages;
using MasterDataFlow.Network;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Genetic;
using MasterDataFlow.Trading.Tester;
using Trady.Analysis;
using Trady.Analysis.Extension;
using Trady.Analysis.Indicator;
using Trady.Core;
using Trady.Core.Infrastructure;
using Trady.Importer;
using Trady.Importer.Csv;
using DirectionTester = MasterDataFlow.Trading.Genetic.DirectionTester;

namespace MasterDataFlow.Trading.Ui
{
    public partial class Form1 : Form
    {
        private TradingChart _tradingChart;

        public Form1()
        {
            InitializeComponent();

            _tradingChart = new TradingChart(tradingChart);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            using (var remote = new RemoteEnvironment())
            {
                await Execute(remote.CommandWorkflow);
            }

        }

        private int _iteration = 0;
        private TradingDataObject _dataObject = null;

        public async Task Execute(CommandWorkflow commandWorkflow)
        {
            await Task.Factory.StartNew(async () => {

                using (var @event = new ManualResetEvent(false))
                {
                    _dataObject = await CreateDataObject();

                    SetChartPrices();

                    var tradingItem = LoadItem(_dataObject.ItemInitData);
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
                                SetText(tbIteration, (_iteration).ToString("D"));

                                sw.Stop();
                                SetText(tbSpeed, (sw.ElapsedMilliseconds).ToString("F10"));
                                sw = System.Diagnostics.Stopwatch.StartNew();

                                var best = (TradingItem)endCycleMessage.Data.Best;
                                DisplayBest(best);

                            }
                        };

                    commandWorkflow.Start<TradingCommand>(_dataObject);
                    @event.WaitOne(1000000);

                }

            });


            //using (var @event = new ManualResetEvent(false))
            //{
            //    TradingDataObject dataObject = await CreateDataObject();
            //    var instancesCount = 10;

            //    commandWorkflow.MessageRecieved += 
            //        (key, message) => {
            //            if (message is StopCommandMessage stopMessage)
            //            {
            //                var best = (TradingItem)(stopMessage.Data as GeneticInfoDataObject).Best;
            //                DisplayBest(best, dataObject);
            //                @event.Set();
            //            }

            //            if (message is GeneticEndCycleMessage endCycleMessage)
            //            {
            //                var best = (TradingItem)endCycleMessage.Data.Best;
            //                DisplayBest(best, dataObject);
            //            }
            //        };

            //   commandWorkflow.Start<TradingCommand>(dataObject);
            //    @event.WaitOne(1000000);

            //    //var theBests = new List<VariableCrossoverGeneticItem>();
            //    //var completedInstances = 0;
            //    //commandWorkflow.MessageRecieved += (key, message) =>
            //    //{
            //    //    Console.WriteLine("Response from {0}", key);
            //    //    var stopMessage = message as StopCommandMessage;
            //    //    if (stopMessage != null)
            //    //    {
            //    //        var best = (VariableCrossoverGeneticItem)(stopMessage.Data as GeneticStopDataObject).Best;
            //    //        lock (this)
            //    //        {
            //    //            theBests.Add(best);
            //    //            ++completedInstances;
            //    //            if (completedInstances == instancesCount)
            //    //            {
            //    //                @event.Set();
            //    //            }
            //    //        }
            //    //    }
            //    //};

            //    //for (int i = 0; i < instancesCount; i++)
            //    //{
            //    //    commandWorkflow.Start<VariableCrossoverGeneticCommand>(dataObject);
            //    //}
            //    //@event.WaitOne(1000000);
            //    //Console.WriteLine("The bests");
            //    //foreach (var theBestItem in theBests)
            //    //{
            //    //    PrintTheBest(theBestItem);
            //    //}
            //    //Console.WriteLine("The best fitness is {0}", theBests.Max(t => t.Fitness));

            //    //dataObject = CreateTravelingSalesmanProblemInitData();
            //    ////dataObject.RepeatCount = 200;
            //    //dataObject.InitPopulation = new List<int[]>();
            //    //dataObject.InitLengths = new List<int[]>();
            //    //for (int i = 0; i < theBests.Count; i++)
            //    //{
            //    //    dataObject.InitPopulation.Add(theBests[i].Values);
            //    //    dataObject.InitLengths.Add(theBests[i].Lengths);
            //    //}
            //    //@event.Reset();
            //    //theBests.Clear();
            //    //instancesCount = 1;
            //    //completedInstances = 0;
            //    //commandWorkflow.Start<VariableCrossoverGeneticCommand>(dataObject);
            //    //@event.WaitOne(1000000);
            //    //Console.WriteLine("The bests");
            //    //foreach (var theBestItem in theBests)
            //    //{
            //    //    PrintTheBest(theBestItem);
            //    //}
            //}

        }

        private void DisplayBest(TradingItem neuronItem, bool isSaveBest = true)
        {
            double fitness = neuronItem.Fitness;

            var indicators = GetIndicators(neuronItem);
            SetText(tbIndicators, indicators);

            SetText(tbFitness, (fitness).ToString("F10"));
            SetText(tbStopLoss, neuronItem.StopLoss.ToString("F10"));

            var stories = new List<Story>();
            if (neuronItem.TrainingTesterResult != null)
            {
                SetText(tbTrainingMinusCount, (neuronItem.TrainingTesterResult.MinusCount).ToString("D"));
                SetText(tbTrainingOrderCount, (neuronItem.TrainingTesterResult.OrderCount).ToString("D"));
                SetText(tbTrainingPlusCount, (neuronItem.TrainingTesterResult.PlusCount).ToString("D"));
                SetText(tbTrainingProfit, (neuronItem.TrainingTesterResult.Profit).ToString("F10"));
                SetText(tbTrainingDiff, (neuronItem.TrainingTesterResult.MinEquity).ToString("F10"));
                stories.AddRange(neuronItem.TrainingTesterResult.Stories);
            }
            if (neuronItem.ValidationTesterResult != null) { 
                stories.AddRange(neuronItem.ValidationTesterResult.Stories);
            }

            var dll = TradingCommand.CreateNeuronDll(_dataObject, neuronItem);
            var tester = new DirectionTester(dll, neuronItem, _testData);
            TesterResult testResult = tester.Run();
            SetText(tbPredictionProfit, (testResult.Profit).ToString("F10"));
            SetText(tbPredictionOrderCount, (testResult.OrderCount).ToString("D"));
            SetText(tbPredictionDiff, (testResult.MinEquity).ToString("F10"));
            SetText(tbPredictionMinusCount, (testResult.MinusCount).ToString("D"));
            SetText(tbPredictionPlusCount, (testResult.PlusCount).ToString("D"));

            stories.AddRange(testResult.Stories);

            SetChartHistory(tradingChart, stories);


            if (isSaveBest)
                SaveBest(neuronItem, testResult);
        }

        private string GetIndicators(TradingItem neuronItem)
        {
            var names = new List<string>();
            for (int i = 0; i < neuronItem.InitData.IndicatorNumber; i++)
            {
                var indicatorIndex = (int)neuronItem.GetIndicatorIndex(i);
                var name = _testData.Indicators[indicatorIndex].Name;
                names.Add(name);
            }
            return string.Join(",", names);
        }

        private delegate void SetTextCallback(TextBox textBox, string text);

        private void SetText(TextBox textBox, string text)
        {
            if (textBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { textBox, text });
            }
            else
            {
                textBox.Text = text;
            }
        }


        private delegate void SetDateTimePickerCallback(DateTimePicker control, DateTime value);

        private void SetDateTimePicker(DateTimePicker control, DateTime value)
        {
            if (control.InvokeRequired)
            {
                SetDateTimePickerCallback d = new SetDateTimePickerCallback(SetDateTimePicker);
                this.Invoke(d, new object[] { control, value });
            }
            else
            {
                control.Value = value;
            }
        }

        private delegate void SetChartHistoryCallBack(Chart chart, IEnumerable<Story> stories);

        private void SetChartHistory(Chart chart, IEnumerable<Story> stories)
        {
            if (chart.InvokeRequired)
            {
                SetChartHistoryCallBack d = new SetChartHistoryCallBack(SetChartHistory);
                this.Invoke(d, new object[] { chart, stories });
            }
            else
            {
                _tradingChart.SetStories(stories);
            }
        }

        private delegate void SetChartPricesCallBack();

        private void SetChartPrices()
        {
            if (tradingChart.InvokeRequired)
            {
                SetChartPricesCallBack d = new SetChartPricesCallBack(SetChartPrices);
                this.Invoke(d, new object[] {  });
            }
            else
            {
                _tradingChart.SetPrices(_tradingBars);
            }
        }

        private Bar[] CandlesToBars(IEnumerable<IOhlcv> candles)
        {
            var result = candles.Select(t => new Bar
            {
                Close = t.Close,
                High = t.High,
                Low = t.Low,
                Open = t.Open,
                Volume = t.Volume,
                Time = t.DateTime.DateTime,
            }).ToArray();
            return result;
        }

        private const int IndicatorsOffset = 50;

        private async Task<TradingDataObject> CreateDataObject()
        {
            var itemInitData = LoadItemInitData();

            await LoadInputData(itemInitData);

            var result = new TradingDataObject(itemInitData, _trainingData, _validationData, 
                100 * (int)nudPopulationFactor.Value, 33 * (int)nudPopulationFactor.Value);

            DisplayProgressChart();

            return result;
        }

        private void DisplayProgressChart()
        {
            //chartProgress.DataBindTable();
        }

        private IEnumerable<IOhlcv> _candles = null;
        private Bar[] _tradingBars = null;
        private List<LearningDataIndicator> _indicators = null;
        private LearningData _trainingData = null;
        private LearningData _validationData = null;
        private LearningData _testData = null;

        private async Task LoadInputData(TradingItemInitData itemInitData)
        {
            var csvImporter = new CsvImporter(@"Data\output.csv", new CultureInfo("en-US"));
            _candles = await csvImporter.ImportAsync("fb");

            _tradingBars = CandlesToBars(_candles);

            CreateIndicatorsValues();

            SetDataBoundaris(itemInitData);
        }

        private void CreateIndicatorsValues()
        {
            _indicators = new List<LearningDataIndicator>();

            // RSI
            IReadOnlyList<AnalyzableTick<decimal?>> rsi = _candles.Rsi(14);
            AddIndicator("RSI", rsi);

            // Parabolic SAR
            var psar = _candles.Sar(0.02M, 0.2M);
            AddIndicator("SAR", psar);

            // EMA
            var ema5 = _candles.Ema(5);
            AddIndicator("EMA 5", ema5);
            var ema10 = _candles.Ema(10);
            AddIndicator("EMA 10", ema10);
            var ema15 = _candles.Ema(15);
            AddIndicator("EMA 15", ema15);
            var ema20 = _candles.Ema(20);
            AddIndicator("EMA 20", ema20);

            // MACD
            var macds = _candles.Macd(12, 26, 9);

            AddIndicator("MACD Histogram", macds.Select(t => (float)t.Tick.MacdHistogram).ToArray(), macds.Select(t => t.DateTime.Value.DateTime).ToArray());
            AddIndicator("MACD SignalLine", macds.Select(t => (float)t.Tick.SignalLine).ToArray(), macds.Select(t => t.DateTime.Value.DateTime).ToArray());
            AddIndicator("MACD Line", macds.Select(t => (float)t.Tick.MacdLine).ToArray(), macds.Select(t => t.DateTime.Value.DateTime).ToArray());
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
            var validationDays = days * 20 / 100;
            var testDays = days * 20 / 100;

            var startValidationDate = startTrainingDate.AddDays(trainingDays);
            var startTestDate = startValidationDate.AddDays(validationDays);

            //var startTestDate = new DateTime(2017, 12, 9);

            _trainingData = CreateLearningData(itemInitData, startTrainingDate, trainingDays);
            _validationData = CreateLearningData(itemInitData, startValidationDate, validationDays);
            _testData = CreateLearningData(itemInitData, startTestDate, testDays);

            SetDateTimePicker(dtpStartTrainingDate, startTrainingDate);
            SetDateTimePicker(dtpStartValidationDate, startValidationDate);
            SetDateTimePicker(dtpStartTestDate, startTestDate);
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
            }

            result.Indicators = indicators.ToArray();
            return result;
        }


        // ----------- Write ---------------

        private Guid lastGuid;

        private void SaveBest(TradingItem item, TesterResult futureResult)
        {
            if (item.TrainingTesterResult == null || item.ValidationTesterResult == null)
                return;

            if (lastGuid != null && lastGuid.ToString() == item.Guid.ToString())
                return;


            using (StreamWriter writer = new StreamWriter("genetic.save"))
            {
                WriteNew(writer, _indicators, item);
            }

            if (!(
                item.TrainingTesterResult.Profit > 0 && item.ValidationTesterResult.Profit > 0
                && futureResult.Profit > 0)
                ) return;


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


        public void WriteNew(TextWriter writer, List<LearningDataIndicator> indicators, TradingItem item)
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

            for (var i = 0; i < item.InitData.IndicatorNumber; i++)
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

        public TradingItem LoadItem(TradingItemInitData itemInitData)
        {
            if (!File.Exists("genetic.save"))
                return null;

            StreamReader reader = new StreamReader("genetic.save");
            try
            {

                XElement root = XElement.Load(reader, LoadOptions.None);
                var item = CreateItem(root, itemInitData);
                ReadItemValues(root, item);

                DisplayBest(item);
                return item;
            }
            finally
            {
                reader.Close();
            }
        }

        public TradingItemInitData LoadItemInitData()
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


        private void ReadItemValues(XElement root, TradingItem item)
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
                item.Values[offset] = ParseStringValue(eValue.Value);
                ++offset;
            }

            eValues = itemElement.Element("values");
            foreach (XElement eValue in eValues.Elements("v"))
            {
                item.Values[offset] = ParseStringValue(eValue.Value);
                ++offset;
            }

        }

        private float ParseStringValue(string value)
        {
            var result = Convert.ToDouble(value);
            return (float)result;
        }

        private TradingItem CreateItem(XElement root, TradingItemInitData itemInitData)
        {
            TradingItem item = new TradingItem(itemInitData);
            return item;
        }


        // https://stackoverflow.com/questions/5887292/asp-net-mvc-3-mschart-error-only-1-y-values-can-be-set-for-this-data-series
        // https://msdn.microsoft.com/en-us/library/dd456730.aspx
        //https://stackoverflow.com/questions/13584061/how-to-enable-zooming-in-microsoft-chart-control-by-using-mouse-wheel
        // google c.net chart zoom
        private async void button2_Click(object sender, EventArgs e)
        {
            var csvImporter = new CsvImporter(@"Data\output.csv", new CultureInfo("en-US"));
            _candles = await csvImporter.ImportAsync("fb");

            _tradingBars = CandlesToBars(_candles);

            _tradingChart.SetPrices(_tradingBars);

            _tradingChart.Test();

            //SetChartMinMaxPrices(_tradingBars);



            //var prices = chartProgress.Series["Prices"];
            //var buy = chartProgress.Series["Buy"];

            ////prices.Points.AddXY(DateTime.Now, 10, 20, 40, 30);

            //foreach (var quote in _tradingBars)
            //{
            //    prices.Points.AddXY(quote.Time, quote.Low, quote.High, quote.Open, quote.Close);
            //}

            //foreach (var quote in _tradingBars)
            //{
            //   var index = buy.Points.AddXY(quote.Time, quote.Low);
            //}
        }

    }
}
