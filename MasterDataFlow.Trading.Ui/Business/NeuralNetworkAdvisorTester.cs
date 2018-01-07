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

namespace MasterDataFlow.Trading.Ui.Business
{
    public class NeuralNetworkAdvisorTester : AbstractTester, ITrader
    {
        private readonly NeuralNetworkAdvisor _advisor;

        public NeuralNetworkAdvisorTester(IAdvisorInfo advisorInfo, ILogger logger, TradingItem tradingItem, NeuronNetwork neuronNetwork, 
            decimal deposit, Bar[] prices, int @from, int length) : base(deposit, prices, @from, length)
        {
            var neuron = NeuronNetwork.CreateNeuronDll(neuronNetwork, tradingItem);
            _advisor = new NeuralNetworkAdvisor(advisorInfo, this, logger, neuron, tradingItem);
        }

        #region AbstractTester

        protected override void OnTick()
        {
            _advisor.Tick(CurrentTime, CurrentPrice);
        }
        #endregion

        #region ITrader
        public bool IsSellOrderExists()
        {
            throw new NotImplementedException();
        }

        public Operationtatus CloseSellOrder()
        {
            throw new NotImplementedException();
        }

        public bool IsBuyOrderExists()
        {
            throw new NotImplementedException();
        }

        public Operationtatus BuyOrder()
        {
            throw new NotImplementedException();
        }

        public Operationtatus CloseBuyOrder()
        {
            throw new NotImplementedException();
        }

        public Operationtatus SellOrder()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
