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
using MasterDataFlow.Trading.IO;
using MasterDataFlow.Trading.Shared.Data;
using MasterDataFlow.Trading.Tester;
using Microsoft.CodeAnalysis.CSharp;
using Trady.Analysis;
using Trady.Analysis.Extension;
using Trady.Core.Infrastructure;
using Trady.Importer.Csv;
using DirectionTester = MasterDataFlow.Trading.Genetic.DirectionTester;

namespace MasterDataFlow.Trading.Ui.Business.Teacher
{
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

    public class BaseCommandController
    {
        private readonly int _processorCount;

        private readonly Writer _writer;
        private readonly DataProvider _dataProvider;

        public BaseCommandController(DataProvider dataProvider, int processorCount)
        {
            _dataProvider = dataProvider;
            _processorCount = processorCount;
            _writer = new Writer(new InputDataCollection());
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

        #region Properties
        public int PopulationFactor { get; set; } = 1;

        public TradingDataObject DataObject
        {
            get { return _dataObject; }
        }

        public LearningData TestData => _dataProvider.TestData;

        #endregion

        #region Events

        public event DisplayBestHandler DisplayBestEvent;
        public event IterationEndHandler IterationEndEvent;

        #endregion
        private async Task ExecuteCommand(CommandWorkflow commandWorkflow)
        {
            await Task.Factory.StartNew(async () => {

                using (var @event = new ManualResetEvent(false))
                {
                    try
                    {
                        await _dataProvider.LoadData();
                        _dataObject = CreateDataObject();

                        var tradingItem = _dataProvider.ReadTradingItem();
                        if (tradingItem != null)
                        {
                            DisplayBest(tradingItem);
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

        private TradingDataObject CreateDataObject()
        {
            var result = new TradingDataObject(_dataProvider.ItemInitData, _dataProvider.Trainings.ToArray(),
                100 * PopulationFactor, 33 * PopulationFactor, _processorCount);

            return result;
        }



        // external 

        private void IterationEnd(int iteration, long elapsedMilliseconds)
        {
            IterationEndEvent?.Invoke(this, new IterationEndArgs(iteration, elapsedMilliseconds));
        }

        private void DisplayBest(TradingItem neuronItem, bool isSaveBest = true)
        {
            DisplayBestEvent?.Invoke(this, new DisplayBestArgs(neuronItem));

            var dll = NeuronNetwork.CreateNeuronDll(_dataObject.ItemInitData.NeuronNetwork, neuronItem);
            var tester = new DirectionTester(dll, neuronItem, _dataProvider.TestData);
            var testResult = tester.Run();

            if (isSaveBest)
                _writer.SaveBest(neuronItem, testResult);

        }
    }
}
