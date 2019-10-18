using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Shared.Data;
using Trady.Importer.Csv;

namespace MasterDataFlow.MetaTrader
{
    public static class TradingComparing
    {
        public static List<InputValues> LoadTrainingData(string filename, bool isNormalize)
        {
            var csvImporter = new CsvImporter(filename, new CultureInfo("en-US"));
            var candles = (csvImporter.ImportAsync("fb")).Result;
            var tradingBars = Helper.CandlesToBars(candles);

            var inputData = new InputDataCollection();
            var inputValues = new List<InputValues>();
            var inputs = inputData.GetInputs();
            foreach (var input in inputs)
            {
                var values = input.GetValues(tradingBars);
                inputValues.Add(values);
                if (isNormalize)
                {
                    var scaler = new MinMaxScaler(input.GetMin(), input.GetMax());
                    scaler.Transform(values);
                }
            }

            return inputValues;
        }

    }
}
