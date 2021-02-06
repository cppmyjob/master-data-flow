using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MasterDataFlow.Trading.Genetic;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Ui
{
    public partial class TradingUserInfo : UserControl
    {
        public class Data
        {
            public double Fitness { get; set; }
            public double StopLoss { get; set; }
            public string Indicators { get; set; }
            public FinalResult FinalResult { get; set; }
            public TesterResult TestResult { get; set; }

        }

        public TradingUserInfo()
        {
            InitializeComponent();
        }

        public void SetData(Data data)
        {
            SetText(tbIndicators, data.Indicators);
            SetText(tbFitness, data.Fitness.ToString("F10"));
            SetText(tbStopLoss, data.StopLoss.ToString("F10"));

            if (data.FinalResult != null)
            {
                SetText(tbTrainingMinusCount, (data.FinalResult.TrainingTesterResult.Sum(t => t.MinusCount)).ToString("D"));
                SetText(tbTrainingOrderCount, (data.FinalResult.TrainingTesterResult.Sum(t => t.OrderCount)).ToString("D"));
                SetText(tbTrainingPlusCount, (data.FinalResult.TrainingTesterResult.Sum(t => t.PlusCount)).ToString("D"));
                SetText(tbTrainingProfit, (data.FinalResult.TrainingTesterResult.Sum(t => t.Profit)).ToString("F10"));
                SetText(tbTrainingDiff, (data.FinalResult.TrainingTesterResult.Average(t => t.MinEquity)).ToString("F10"));
            }

            SetText(tbPredictionProfit, (data.TestResult.Profit).ToString("F10"));
            SetText(tbPredictionOrderCount, (data.TestResult.OrderCount).ToString("D"));
            SetText(tbPredictionDiff, (data.TestResult.MinEquity).ToString("F10"));
            SetText(tbPredictionMinusCount, (data.TestResult.MinusCount).ToString("D"));
            SetText(tbPredictionPlusCount, (data.TestResult.PlusCount).ToString("D"));

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

    }
}
