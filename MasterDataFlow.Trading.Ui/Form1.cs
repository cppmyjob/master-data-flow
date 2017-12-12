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
using System.Xml.Linq;
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

        private int _iteration = 0;
        private TradingDataObject _dataObject = null;

        public async Task Execute(CommandWorkflow commandWorkflow)
        {
            await Task.Factory.StartNew(async () => {

                using (var @event = new ManualResetEvent(false))
                {
                    _dataObject = await CreateDataObject();

                    var initItem = ReadLast();
                    if (initItem != null)
                    {
                        _dataObject.InitPopulation = new List<float[]>();
                        _dataObject.InitPopulation.Add(initItem.Values);
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
            SetText(tbStopLoss, neuronItem.StopLoss.ToString("D"));
            if (neuronItem.TrainingTesterResult != null)
            {
                SetText(tbTrainingMinusCount, (neuronItem.TrainingTesterResult.MinusCount).ToString("D"));
                SetText(tbTrainingOrderCount, (neuronItem.TrainingTesterResult.OrderCount).ToString("D"));
                SetText(tbTrainingPlusCount, (neuronItem.TrainingTesterResult.PlusCount).ToString("D"));
                SetText(tbTrainingProfit, (neuronItem.TrainingTesterResult.Profit).ToString("F10"));
                SetText(tbTrainingDiff, (neuronItem.TrainingTesterResult.MinEquity).ToString("F10"));
            }

            var dll = TradingCommand.CreateNeuronDll(_dataObject, neuronItem);
            var tester = new DirectionTester(dll, neuronItem, HistoryWindowLength, _testData);
            TesterResult testResult = tester.Run();
            SetText(tbPredictionProfit, (testResult.Profit).ToString("F10"));
            SetText(tbPredictionOrderCount, (testResult.OrderCount).ToString("D"));
            SetText(tbPredictionDiff, (testResult.MinEquity).ToString("F10"));
            SetText(tbPredictionMinusCount, (testResult.MinusCount).ToString("D"));
            SetText(tbPredictionPlusCount, (testResult.PlusCount).ToString("D"));
            if (isSaveBest)
                SaveBest(neuronItem, testResult);
        }

        private string GetIndicators(TradingItem neuronItem)
        {
            var names = new List<string>();
            for (int i = 0; i < TradingItem.INDICATOR_NUMBER; i++)
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

        private const int HistoryWindowLength = 24;
        private const int IndicatorsOffset = 50;

        private async Task<TradingDataObject> CreateDataObject()
        {
            await LoadInputData();


            var result = new TradingDataObject(_trainingData, _validationData, HistoryWindowLength,
                100 * (int)nudPopulationFactor.Value, 33 * (int)nudPopulationFactor.Value);
            result.RepeatCount = 300;


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
            //var macds = _candles.Macd(12, 26, 9);

            //var macd = new MovingAverageConvergenceDivergence(_candles, 12, 26, 9);
            //macd.Compute(null, null);

            //AddIndicator("MACD Histogram", macds.Select(t => (float) t.Tick.MacdHistogram).ToArray(), macds.Select(t => t.DateTime.Value.DateTime).ToArray());
            //AddIndicator("MACD SignalLine", macds.Select(t => (float)t.Tick.SignalLine).ToArray(), macds.Select(t => t.DateTime.Value.DateTime).ToArray());
            //AddIndicator("MACD Line", macds.Select(t => (float)t.Tick.MacdLine).ToArray(), macds.Select(t => t.DateTime.Value.DateTime).ToArray());

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

            SetDateTimePicker(dtpStartTrainingDate, startTrainingDate);
            SetDateTimePicker(dtpStartValidationDate, startValidationDate);
            SetDateTimePicker(dtpStartTestDate, startTestDate);
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
                Write(writer, item);
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
            string line = "Guid; ";
            line += "Fitness; ";
            line += "StopLoss; ";
            line += AddHeaderTestResult("tr");
            line += AddHeaderTestResult("pr");
            line += AddHeaderTestResult("fr");
            writer.WriteLine(line);
        }

        private string AddHeaderTestResult(string prefix)
        {
            string line = "";
            line += prefix + ".Profit; ";
            line += prefix + ".OrderCount; ";
            line += prefix + ".PlusCount; ";
            line += prefix + ".MinusCount; ";
            line += prefix + ".MaxEquity; ";
            line += prefix + ".MinEquity; ";
            line += prefix + ".PlusEquityCount; ";
            line += prefix + ".MinusEquityCount; ";
            return line;
        }

        private void AddItemToFile(StreamWriter writer, TradingItem item, TesterResult futureResult)
        {
            string line = item.Guid.ToString() + "; ";
            line += item.Fitness.ToString("F10") + "; ";
            line += item.StopLoss.ToString("D") + "; ";
            line = SaveTestResult(item.TrainingTesterResult, line);
            line = SaveTestResult(item.ValidationTesterResult, line);
            line = SaveTestResult(futureResult, line);

            writer.WriteLine(line);
        }

        private string SaveTestResult(TesterResult tr, string line)
        {
            line += tr.Profit.ToString("F10") + "; ";
            line += tr.OrderCount.ToString("D") + "; ";
            line += tr.PlusCount.ToString("D") + "; ";
            line += tr.MinusCount.ToString("D") + "; ";
            line += tr.MaxEquity.ToString("F10") + "; ";
            line += tr.MinEquity.ToString("F10") + "; ";
            line += tr.PlusEquityCount.ToString("D") + "; ";
            line += tr.MinusEquityCount.ToString("D") + "; ";
            return line;
        }

        private void SaveBestItem(TradingItem item)
        {
            StreamWriter writer = new StreamWriter("Best/" + item.Guid.ToString() + ".save");
            try
            {
                Write(writer, item);
            }
            finally
            {
                writer.Close();
            }
        }

        public void Write(TextWriter writer, TradingItem item)
        {
            XElement root = new XElement("Genetic");
            root.Add(new XElement("ItemsCount", _dataObject.CellInitData.ItemsCount));
            root.Add(new XElement("SurviveCount", _dataObject.CellInitData.SurviveCount));
            root.Add(new XElement("ValuesCount", _dataObject.CellInitData.ValuesCount));

            item.Write(root);
            root.Save(writer);
        }

        public TradingItem ReadLast()
        {
            if (!File.Exists("genetic.save"))
                return null;

            StreamReader reader = new StreamReader("genetic.save");
            try
            {
                GeneticItemInitData initData = new GeneticItemInitData
                                               {
                    Count = _dataObject.CellInitData.ValuesCount,
                    IsAddHistory = false,
                    YearOfBorn = 0,
                };

                TradingItem item = new TradingItem(initData);

                XElement root = XElement.Load(reader, LoadOptions.None);
                item.Read(root);
                DisplayBest(item);
                return item;

            }
            finally
            {
                reader.Close();
            }

        }
    }
}
