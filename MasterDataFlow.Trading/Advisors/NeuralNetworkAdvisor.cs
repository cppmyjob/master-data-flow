using System;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Intelligence.Neuron;
using MasterDataFlow.Intelligence.Neuron.SimpleNeuron;
using MasterDataFlow.Trading.Genetic;
using MasterDataFlow.Trading.Interfaces;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Advisors
{

    public  class NeuralNetworkAdvisor : BaseAdvisor
    {
        private readonly ITradingLogger _logger;
        private readonly ISimpleNeuron _neuron;
        private readonly TradingItem _tradingItem;
        private readonly IInputDataCollection _inputDataCollection;

        public NeuralNetworkAdvisor(IAdvisorInfo advisorInfo, 
            ITrader trader, ITradingLogger logger, ISimpleNeuron neuron, IInputDataCollection inputDataCollection, TradingItem tradingItem) : base(advisorInfo, trader, logger)
        {
            _logger = logger;
            _neuron = neuron;
            _tradingItem = tradingItem;
            _inputDataCollection = inputDataCollection;
        }

        protected override AdvisorSignal GetSignal(DateTime time, decimal price)
        {
            if (!Info.LastSignalTime.HasValue || IsDifferentHour(Info.LastSignalTime.Value))
                return CalculateSignal();
            return AdvisorSignal.Skip;
        }

        private bool IsDifferentHour(DateTime last)
        {
            var currentTime = DateTime.Now;
            if (currentTime - last >= new TimeSpan(1, 0, 0))
            {
                return true;
            }
            return currentTime.Hour != last.Hour;
        }

        private AdvisorSignal CalculateSignal()
        {
            var inputs =
                new float[_tradingItem.InitData.HistoryWidowLength * _tradingItem.InitData.InputData.Indicators.IndicatorNumber +
                          (TradingItemInitData.IS_RECURRENT ? TradingItemInitData.OUTPUT_NUMBER : 0)];
           


            var outputs = _neuron.NetworkCompute(inputs);
            var isBuySignal = outputs[0] > 0.5F;
            var isSellSignal = outputs[1] > 0.5F;
            var isHoldSignal = outputs[2] > 0.5F;

            var isHold = !isBuySignal && !isSellSignal && isHoldSignal;
            var isBuy = isBuySignal && !isSellSignal && !isHoldSignal;
            var isSell = !isBuySignal && isSellSignal && !isHoldSignal;
            var isClose = !(isHold || isBuy || isSell);

            if (isHold)
                return AdvisorSignal.Hold;
            if (isBuy)
                return AdvisorSignal.Buy;
            if (isSell)
                return AdvisorSignal.Sell;

            return AdvisorSignal.Close;
        }
    }

}
