using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MasterDataFlow.Trading.Genetic;
using MasterDataFlow.Trading.Interfaces;
using MasterDataFlow.Trading.Tester;


namespace MasterDataFlow.Trading.IO
{
    public class Writer
    {
        private readonly IInputDataCollection _inputData;

        public Writer(IInputDataCollection inputData)
        {
            _inputData = inputData;
        }

        private Guid lastGuid;

        public void SaveBest(TradingItem item, TesterResult futureResult)
        {
            lock (this)
            {

                if (item.FinalResult == null)
                    return;

                if (lastGuid != null && lastGuid.ToString() == item.Guid.ToString())
                    return;


                using (StreamWriter writer = new StreamWriter("genetic.save"))
                {
                    WriteNew(writer, item);
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
            line += "FitnessOriginal, ";
            line += "FitnessOriginalStdPercent, ";
            line += "FitnessOriginalStd, ";
            line += "FitnessExpectedValue, ";
            line += "FitnessZigZag, ";
            line += "FitnessProfit, ";
            line += "FitnessPlusMinusOrdersRatio, ";
            line += "FitnessPlusMinusEquityRatio, ";
            line += "FitnessTradingCount, ";
            line += "FitnessOrderCount, ";
            line += "StopLoss, ";
            line += AddHeaderTestResult("tr");
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
            line += item.FitnessOriginal.ToString("F10") + ", ";
            line += GetStdPercent(item).ToString("F2") + ", ";
            line += item.FinalResult.Std.ToString("F10") + ", ";
            line += item.FitnessExpectedValue.ToString("F10") + ", ";
            line += item.FitnessZigZag.ToString("F10") + ", ";
            line += item.FitnessProfit.ToString("F10") + ", ";
            line += item.FitnessPlusMinusOrdersRatio.ToString("F10") + ", ";
            line += item.FitnessPlusMinusEquityRatio.ToString("F10") + ", ";
            line += item.FitnessTradingCount.ToString("F10") + ", ";
            line += item.FitnessOrderCount.ToString("F10") + ", ";
            line += item.StopLoss.ToString("F10") + ", ";
            line = SaveTestResult(item.FinalResult.TrainingTesterResult, line);
            line = SaveTestResult(new []{futureResult}, line);

            writer.WriteLine(line);
        }

        private double GetStdPercent(TradingItem item)
        {
            return item.FinalResult.Std * 100 / item.FitnessOriginal;
        }

        private string SaveTestResult(TesterResult[] tr, string line)
        {
            line += tr.Sum(t => t.Profit).ToString("F10") + ", ";
            line += tr.Sum(t => t.OrderCount).ToString("D") + ", ";
            line += tr.Sum(t => t.PlusCount).ToString("D") + ", ";
            line += tr.Sum(t => t.MinusCount).ToString("D") + ", ";
            line += tr.Average(t => t.MaxEquity).ToString("F10") + ", ";
            line += tr.Average(t => t.MinEquity).ToString("F10") + ", ";
            line += tr.Sum(t => t.PlusEquityCount).ToString("D") + ", ";
            line += tr.Sum(t => t.MinusEquityCount).ToString("D") + ", ";
            return line;
        }

        private void SaveBestItem(TradingItem item)
        {
            StreamWriter writer = new StreamWriter("Best/" + item.Guid.ToString() + ".save");
            try
            {
                WriteNew(writer, item);
            }
            finally
            {
                writer.Close();
            }
        }

        private void WriteNew(TextWriter writer, TradingItem item)
        {
            XElement root = new XElement("genetic");

            item.InitData.Write(root);

            WriteItem(root, item);
            root.Save(writer);
        }


        private void WriteItem(XElement root, TradingItem item)
        {
            var itemElement = new XElement("item");
            root.Add(itemElement);

            itemElement.Add(new XElement("fitness", item.Fitness.ToString(CultureInfo.InvariantCulture)));
            itemElement.Add(new XElement("fitnessZigZag", item.FitnessZigZag.ToString(CultureInfo.InvariantCulture)));
            itemElement.Add(new XElement("fitnessProfit", item.FitnessProfit.ToString(CultureInfo.InvariantCulture)));
            itemElement.Add(new XElement("fitnessExpectedValue", item.FitnessExpectedValue.ToString(CultureInfo.InvariantCulture)));
            itemElement.Add(new XElement("fitnessPlusMinusOrdersRatio", item.FitnessPlusMinusOrdersRatio.ToString(CultureInfo.InvariantCulture)));
            itemElement.Add(new XElement("fitnessPlusMinusEquityRatio", item.FitnessPlusMinusEquityRatio.ToString(CultureInfo.InvariantCulture)));
            itemElement.Add(new XElement("fitnessTradingCount", item.FitnessTradingCount.ToString(CultureInfo.InvariantCulture)));
            itemElement.Add(new XElement("fitnessOrderCount", item.FitnessOrderCount.ToString(CultureInfo.InvariantCulture)));
            itemElement.Add(new XElement("fitnessOriginal", item.FitnessOriginal.ToString(CultureInfo.InvariantCulture)));
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

                v.Add(new XAttribute("name", _inputData.GetInputs()[(int)value].Name));
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


    }
}
