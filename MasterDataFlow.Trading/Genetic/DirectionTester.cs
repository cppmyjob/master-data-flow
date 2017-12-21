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
        private readonly LearningData _learningData;
        private const decimal START_DEPOSIT = 100000;

        public DirectionTester(GeneticNeuronDLL1 dll, TradingItem tradingItem, LearningData learningData) 
            : base(START_DEPOSIT, learningData.Prices, 0, learningData.Prices.Length)
        {
            _dll = dll;
            _tradingItem = tradingItem;
            _learningData = learningData;
        }

        private float[] _inputs;

        protected override Direction GetDirection(int index)
        {
            if (_inputs == null)
            {
              _inputs = new float[_tradingItem.InitData.HistoryWidowLength * _tradingItem.InitData.IndicatorNumber];
            }

            for (int i = 0; i < _tradingItem.InitData.IndicatorNumber; i++)
            {
                var indicatorIndex = (int)_tradingItem.GetIndicatorIndex(i);
                var indicatorValues = _learningData.Indicators[indicatorIndex].Values;
                Array.Copy(indicatorValues, index, _inputs, _tradingItem.InitData.HistoryWidowLength * i, _tradingItem.InitData.HistoryWidowLength);
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


        protected override decimal GetStopLoss()
        {
            return (decimal)_tradingItem.StopLoss;
        }
    }
}
