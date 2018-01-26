using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Intelligence.Neuron;
using MasterDataFlow.Intelligence.Neuron.SimpleNeuron;
using MasterDataFlow.Trading.Advisors;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Genetic;
using MasterDataFlow.Trading.Interfaces;
using MasterDataFlow.Trading.Tester;
using MasterDataFlow.Trading.Ui.Business.Data;

namespace MasterDataFlow.Trading.Ui.Business.Advisor
{
    public class NeuralNetworkAdvisorTester : SignleOrderTester, ITrader
    {
        private readonly TradingItem _tradingItem;
        private readonly Bar[] _prices;
        private readonly NeuralNetworkAdvisor _advisor;
        private readonly InputDataCollection _inputData = new InputDataCollection();

        public NeuralNetworkAdvisorTester(IAdvisorInfo advisorInfo, ITradingLogger logger, TradingItem tradingItem, NeuronNetwork neuronNetwork, 
            decimal deposit, Bar[] prices, int @from, int length) : base(deposit, prices, @from, length)
        {
            _tradingItem = tradingItem;
            _prices = prices;
            var neuron = NeuronNetwork.CreateNeuronDll(neuronNetwork, tradingItem);
            _advisor = new NeuralNetworkAdvisor(advisorInfo, this, logger, neuron, _inputData, tradingItem);
        }

        #region AbstractTester

        protected override void OnTick()
        {
            _advisor.Tick(CurrentTime, CurrentPrice);
        }

        protected override decimal GetStopLoss()
        {
            return (decimal)_tradingItem.StopLoss;
        }

        #endregion

        #region ITrader

        public Bar[] GetBars(int index, int length)
        {
            var result = new Bar[length];
            var from = CurrentBar - index - length;
            Array.Copy(_prices, from, result, 0, length);
            return result;
        }

        public int GetIndicatorsOffset()
        {
            return Constants.IndicatorsOffset;
        }

        public bool IsSellOrderExists()
        {
            return _currentOrder?.Type == OrderType.Sell;
        }

        public Operationtatus CloseSellOrder()
        {
            if (_currentOrder.Type == OrderType.Sell)
            {
                CloseOrder(_currentOrder.Ticket);
                _currentOrder = null;
            }
            return Operationtatus.Ok;
        }

        public bool IsBuyOrderExists()
        {
            return _currentOrder?.Type == OrderType.Buy;
        }

        public Operationtatus BuyOrder()
        {
            Buy();
            return Operationtatus.Ok;
        }

        public Operationtatus CloseBuyOrder()
        {
            if (_currentOrder.Type == OrderType.Buy)
            {
                CloseOrder(_currentOrder.Ticket);
                _currentOrder = null;
            }
            return Operationtatus.Ok;
        }

        public Operationtatus SellOrder()
        {
            Sell();
            return Operationtatus.Ok;
        }



        #endregion

    }
}
