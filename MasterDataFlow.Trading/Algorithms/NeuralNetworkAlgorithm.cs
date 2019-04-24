using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Trading.Advisors;
using MasterDataFlow.Trading.Genetic;
using MasterDataFlow.Trading.Interfaces;

namespace MasterDataFlow.Trading.Algorithms
{
    public class NeuralNetworkAlgorithm : ITradingAlgorithm
    {
        private readonly TradingItem _tradingItem;
        private readonly ISimpleNeuron _neuron;

        public NeuralNetworkAlgorithm(TradingItem tradingItem, ISimpleNeuron neuron)
        {
            _tradingItem = tradingItem;
            _neuron = neuron;
        }


        public AlgorithmSignal GetSignal(float[] inputs, float[] outRecurrent)
        {
            var outputs = _neuron.NetworkCompute(inputs);

            if (TradingItemInitData.IS_RECURRENT)
            {
                Array.Copy(outputs, outRecurrent, TradingItemInitData.OUTPUT_NUMBER);
            }

            var isBuySignal = outputs[0] > 0.5F;
            var isSellSignal = outputs[1] > 0.5F;
            var isHoldSignal = outputs[2] > 0.5F;


            var isHold = !isBuySignal && !isSellSignal && isHoldSignal;
            var isBuy = isBuySignal && !isSellSignal && !isHoldSignal;
            var isSell = !isBuySignal && isSellSignal && !isHoldSignal;
            //var isClose = !(isHold || isBuy || isSell);

            if (isHold)
                return AlgorithmSignal.Hold;
            if (isBuy)
                return AlgorithmSignal.Buy;
            if (isSell)
                return AlgorithmSignal.Sell;

            return AlgorithmSignal.Close;
        }
    }
}
