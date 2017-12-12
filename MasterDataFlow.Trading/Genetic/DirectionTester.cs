using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Intelligence.Neuron;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Genetic
{
    public class DirectionTester : Tester.DirectionTester
    {
        private readonly GeneticNeuronDLL1 _dll;
        private readonly TradingItem _tradingItem;
        private readonly int _historyWindowLength;
        private readonly LearningData _learningData;
        private const double START_DEPOSIT = 100000;

        public DirectionTester(GeneticNeuronDLL1 dll, TradingItem tradingItem, int historyWindowLength, LearningData learningData) 
            : base(START_DEPOSIT, learningData.Prices, 0, learningData.Prices.Length)
        {
            _dll = dll;
            _tradingItem = tradingItem;
            _historyWindowLength = historyWindowLength;
            _learningData = learningData;
        }

        private float[] _inputs;

        protected override Direction GetDirection(int index)
        {
            if (_inputs == null)
            {
              _inputs = new float[_historyWindowLength * TradingItem.INDICATOR_NUMBER];
            }

            for (int i = 0; i < TradingItem.INDICATOR_NUMBER; i++)
            {
                var indicatorIndex = (int)_tradingItem.GetIndicatorIndex(i);
                var indicatorValues = _learningData.Indicators[indicatorIndex].Values;
                Array.Copy(indicatorValues, index, _inputs, _historyWindowLength * i, _historyWindowLength);
            }

            var outputs = _dll.NetworkCompute(_inputs);
            var isBuy = outputs[0] > 0.5F;
            var isSell = outputs[1] > 0.5F;
            if (isBuy && isSell)
                return Direction.None;
            if (isBuy)
                return Direction.Up;
            if (isSell)
                return Direction.Down;
            return Direction.None;
        }


        protected override int GetStopLoss()
        {
            return _tradingItem.StopLoss;
        }
    }
}
