using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Genetic;
using MasterDataFlow.Trading.Interfaces;
using MasterDataFlow.Trading.Ui.Business.Data;
using MasterDataFlow.Trading.Ui.Business.IO;
using Trady.Core.Infrastructure;
using Trady.Importer.Csv;

namespace MasterDataFlow.Trading.Ui.Business.Advisor
{
    public class NeuralNetworkAdvisorController : ITradingLogger
    {
        private InputDataCollection _inputData = new InputDataCollection();
        private IReadOnlyList<IOhlcv> _candles;
        private Bar[] _tradingBars;
        private NeuralNetworkAdvisorTester _tester;
        private readonly Reader _reader = new Reader();

        public async Task Run()
        {
            await LoadInputData();

            var advisorInfo = new MemoryAdvisorInfo();

            var initData = _reader.ReadItemInitData();
            var tradingItem = _reader.ReadItem(initData);

            _tester = new NeuralNetworkAdvisorTester(advisorInfo, this, tradingItem, initData.NeuronNetwork, 100000,
                _tradingBars, 0, _tradingBars.Length);
        }

        #region ITradingLogger

        void ITradingLogger.Error(string message)
        {
            
        }

        void ITradingLogger.Error(string message, Exception ex)
        {
            
        }

        void ITradingLogger.Info(string message)
        {
            
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
