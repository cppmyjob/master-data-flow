using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Genetic;
using MasterDataFlow.Trading.Interfaces;
using Trady.Core.Infrastructure;
using Trady.Importer.Csv;

namespace MasterDataFlow.Trading.Ui.Business.Advisor
{
    public class NeuralNetworkAdvisorController : ITradingLogger
    {
        private IReadOnlyList<IOhlcv> _candles;
        private Bar[] _tradingBars;
        private NeuralNetworkAdvisorTester _tester;

        public async Task Run()
        {
            await LoadInputData();

            var advisorInfo = new MemoryAdvisorInfo();

           //_tester = new NeuralNetworkAdvisorTester(advisorInfo, this,)
        }

        #region ITradingLogger

        void ITradingLogger.Error(string message)
        {
            throw new NotImplementedException();
        }

        void ITradingLogger.Error(string message, Exception ex)
        {
            throw new NotImplementedException();
        }

        void ITradingLogger.Info(string message)
        {
            throw new NotImplementedException();
        }

        #endregion



        private async Task LoadInputData()
        {
            var csvImporter = new CsvImporter(@"Data\SBER.csv", new CultureInfo("en-US"));
            _candles = await csvImporter.ImportAsync("fb");

            _tradingBars = Helper.CandlesToBars(_candles);


        }

        void ITradingLogger.Warn(string message)
        {
            throw new NotImplementedException();
        }
    }
}
