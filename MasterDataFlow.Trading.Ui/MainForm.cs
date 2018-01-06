using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MasterDataFlow.Trading.Configs;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Genetic;
using MasterDataFlow.Trading.Tester;
using MasterDataFlow.Trading.Ui.Business;
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
            controller.DisplayBestEvent += ControllerOnDisplayBestEvent;
            controller.DisplayChartPricesEvent += ControllerOnDisplayChartPricesEvent;
            controller.IterationEndEvent += ControllerOnIterationEndEvent;
            controller.SetPeriodsEvent += ControllerOnSetPeriodsEvent;

            await controller.Execute();
        }

        private void ControllerOnSetPeriodsEvent(object sender, PeriodChangedArgs args)
        {
            SetDateTimePicker(dtpStartTrainingDate, args.StartTraining);
            SetDateTimePicker(dtpStartValidationDate, args.StartValidation);
            SetDateTimePicker(dtpStartTestDate, args.StartTest);
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
            if (neuronItem.TrainingTesterResult != null)
            {
                SetText(tbTrainingMinusCount, (neuronItem.TrainingTesterResult.MinusCount).ToString("D"));
                SetText(tbTrainingOrderCount, (neuronItem.TrainingTesterResult.OrderCount).ToString("D"));
                SetText(tbTrainingPlusCount, (neuronItem.TrainingTesterResult.PlusCount).ToString("D"));
                SetText(tbTrainingProfit, (neuronItem.TrainingTesterResult.Profit).ToString("F10"));
                SetText(tbTrainingDiff, (neuronItem.TrainingTesterResult.MinEquity).ToString("F10"));
                stories.AddRange(neuronItem.TrainingTesterResult.Stories);
            }
            if (neuronItem.ValidationTesterResult != null)
            {
                stories.AddRange(neuronItem.ValidationTesterResult.Stories);
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
    }
}
