using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MasterDataFlow.Trading.Configs;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Genetic;
using MasterDataFlow.Trading.Tester;
using MasterDataFlow.Trading.Ui.Business;
using MasterDataFlow.Trading.Ui.Business.Teacher;
using DirectionTester = MasterDataFlow.Trading.Tester.DirectionTester;

namespace MasterDataFlow.Trading.Ui
{
    public partial class MainForm : Form
    {
        private TradingChart _tradingChart;

        public MainForm()
        {
            InitializeComponent();

            _tradingChart = new TradingChart(tradingChart);
        }

        #region Explorer

        private async void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;

            var processorCount = ((KeyValuePair<int, string>)cmbProcessors.SelectedItem).Key;
            var controller = new TradingCommandController(processorCount);
            controller.PopulationFactor = (int)nudPopulationFactor.Value;
            controller.DisplayBestEvent += ControllerOnDisplayBestEvent;
            controller.DisplayChartPricesEvent += ControllerOnDisplayChartPricesEvent;
            controller.IterationEndEvent += ControllerOnIterationEndEvent;
            controller.SetPeriodsEvent += ControllerOnSetPeriodsEvent;

            await controller.Execute();
        }

        private void ControllerOnSetPeriodsEvent(object sender, PeriodChangedArgs args)
        {
            var items = new List<string[]>();
            for (int i = 0; i < args.Trainings.Length; i++)
            {
                var data = args.Trainings[i];
                items.Add( new []{ "Training "+ i, data.StartDateTime.ToString("u") });
            }

            items.Add(new[] { "Test ", args.Test.StartDateTime.ToString("u") });

            SetListView(lvDateRange, items.ToArray());

        }

        private void ControllerOnIterationEndEvent(object sender, IterationEndArgs args)
        {
            SetText(tbIteration, args.Iteration.ToString("D"));
            SetText(tbSpeed, args.ElapsedMilliseconds.ToString("F10"));
        }

        private void ControllerOnDisplayChartPricesEvent(object sender, DisplayChartPricesArgs args)
        {
            var controller = (BaseCommandController)sender;
            Bar[] prices = controller.TradingBars;
            int[] zigZag = controller.ZigZag;
            SetChartPrices(prices, zigZag);
        }

        private void ControllerOnDisplayBestEvent(object sender, DisplayBestArgs args)
        {
            var controller = (BaseCommandController) sender;

            var neuronItem = args.NeuronItem;

            double fitness = neuronItem.Fitness;

            var indicators = GetIndicators(controller, neuronItem);
            SetText(tbIndicators, indicators);

            SetText(tbFitness, (fitness).ToString("F10"));
            SetText(tbStopLoss, neuronItem.StopLoss.ToString("F10"));

            var stories = new List<Story>();
            if (neuronItem.FinalResult != null)
            {
                SetText(tbTrainingMinusCount, (neuronItem.FinalResult.TrainingTesterResult.Sum(t => t.MinusCount)).ToString("D"));
                SetText(tbTrainingOrderCount, (neuronItem.FinalResult.TrainingTesterResult.Sum(t => t.OrderCount)).ToString("D"));
                SetText(tbTrainingPlusCount, (neuronItem.FinalResult.TrainingTesterResult.Sum(t => t.PlusCount)).ToString("D"));
                SetText(tbTrainingProfit, (neuronItem.FinalResult.TrainingTesterResult.Sum(t => t.Profit)).ToString("F10"));
                SetText(tbTrainingDiff, (neuronItem.FinalResult.TrainingTesterResult.Average(t => t.MinEquity)).ToString("F10"));
                stories.AddRange(neuronItem.FinalResult.TrainingTesterResult.SelectMany(t => t.Stories).ToArray());
            }

            var neuron = NeuronNetwork.CreateNeuronDll(controller.DataObject.ItemInitData.NeuronNetwork, neuronItem);
            var tester = new MasterDataFlow.Trading.Genetic.DirectionTester(neuron, neuronItem, controller.TestData);
            TesterResult testResult = tester.Run();
            SetText(tbPredictionProfit, (testResult.Profit).ToString("F10"));
            SetText(tbPredictionOrderCount, (testResult.OrderCount).ToString("D"));
            SetText(tbPredictionDiff, (testResult.MinEquity).ToString("F10"));
            SetText(tbPredictionMinusCount, (testResult.MinusCount).ToString("D"));
            SetText(tbPredictionPlusCount, (testResult.PlusCount).ToString("D"));

            stories.AddRange(testResult.Stories);

            SetChartHistory(tradingChart, stories);
        }

        private string GetIndicators(BaseCommandController controller, TradingItem neuronItem)
        {
            var names = new List<string>();
            for (int i = 0; i < neuronItem.InitData.InputData.Indicators.IndicatorNumber; i++)
            {
                var indicatorIndex = (int)neuronItem.GetIndicatorIndex(i);
                var name = controller.TestData.Indicators[indicatorIndex].Name;
                names.Add(name);
            }
            return string.Join(",", names);
        }

        #endregion

        #region Tester

        private void btnOpenTestFile_Click(object sender, EventArgs e)
        {
            if (ofdOpenTestFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // ofdOpenTestFile.FileName
            }

        }

        #endregion


        // -----------------------------------------------------------------

        private delegate void SetChartPricesCallBack(Bar[] prices, int[] zigZag);

        private void SetChartPrices(Bar[] prices, int[] zigZag)
        {
            if (tradingChart.InvokeRequired)
            {
                var d = new SetChartPricesCallBack(SetChartPrices);
                this.Invoke(d, new object[] { prices, zigZag });
            }
            else
            {
                _tradingChart.SetPrices(prices);
                _tradingChart.SetZigZag(zigZag);
            }
        }

        private delegate void SetChartHistoryCallBack(Chart chart, IEnumerable<Story> stories);

        private void SetChartHistory(Chart chart, IEnumerable<Story> stories)
        {
            if (chart.InvokeRequired)
            {
                var d = new SetChartHistoryCallBack(SetChartHistory);
                this.Invoke(d, new object[] { chart, stories });
            }
            else
            {
                _tradingChart.SetStories(stories);
            }
        }


        private delegate void SetDateTimePickerCallback(DateTimePicker control, DateTime value);

        private void SetDateTimePicker(DateTimePicker control, DateTime value)
        {
            if (control.InvokeRequired)
            {
                var d = new SetDateTimePickerCallback(SetDateTimePicker);
                this.Invoke(d, new object[] { control, value });
            }
            else
            {
                control.Value = value;
            }
        }


        private delegate void SetListBoxCallback(ListBox control, object[] items);

        private void SetListBox(ListBox control, object[] items)
        {
            if (control.InvokeRequired)
            {
                var d = new SetListBoxCallback(SetListBox);
                this.Invoke(d, new object[] { control, items });
            }
            else
            {
                control.Items.Clear();
                control.Items.AddRange(items);
            }
        }

        private delegate void SetListViewCallback(ListView control, string[][] items);

        private void SetListView(ListView control, string[][] items)
        {
            if (control.InvokeRequired)
            {
                var d = new SetListViewCallback(SetListView);
                this.Invoke(d, new object[] { control, items });
            }
            else
            {
                control.Items.Clear();
                for (int i = 0; i < items.Length; i++)
                {
                    var item = control.Items.Add(items[i][0]);
                    item.SubItems.Add(items[i][1]);
                }
            }
        }

        private delegate void SetTextCallback(TextBox textBox, string text);

        private void SetText(TextBox textBox, string text)
        {
            if (textBox.InvokeRequired)
            {
                var d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { textBox, text });
            }
            else
            {
                textBox.Text = text;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //cmbProcessors.Add
            var data = new Dictionary<int, string>
                       {
                           {-1, "No restrctions"}
                       };

            for (var i = 0; i < Environment.ProcessorCount; i++)
            {
                data.Add(i + 1, (i+1).ToString());
            }

            cmbProcessors.DataSource = new BindingSource(data, null);
            cmbProcessors.DisplayMember = "Value";
            cmbProcessors.ValueMember = "Key";

            var commandInitDataConfig = CommandInitDataConfigSection.GetConfig();
            cmbProcessors.SelectedIndex = 0;
            if (commandInitDataConfig != null)
            {
                for (int i = 0; i < cmbProcessors.Items.Count; i++)
                {
                    var item = (KeyValuePair<int, string>)cmbProcessors.Items[i];
                    if (item.Key == commandInitDataConfig.ProcessorCount)
                    {
                        cmbProcessors.SelectedIndex = i;
                        break;
                    }
                }
            }
            
        }

        private void btnTesterStart_Click(object sender, EventArgs e)
        {

        }
    }
}
