using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Intelligence.Neuron;
using MasterDataFlow.Intelligence.Neuron.SimpleNeuron;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Genetic
{
    public class DirectionTester : Tester.DirectionTester
    {
        private readonly ISimpleNeuron _neuron;
        private readonly TradingItem _tradingItem;
        private readonly LearningData _learningData;
        private const decimal START_DEPOSIT = 100000;

        private int _zigZagFitness = 0;

        private float[] previuosOutput = new float[TradingItemInitData.OUTPUT_NUMBER];

        public DirectionTester(ISimpleNeuron neuron, TradingItem tradingItem, LearningData learningData) 
            : base(START_DEPOSIT, learningData.Prices, 0, learningData.Prices.Length)
        {
            _neuron = neuron;
            _tradingItem = tradingItem;
            _learningData = learningData;
        }

        private float[] _inputs;

        public double ZigZagCount
        {
            get
            {

                return _zigZagFitness;
            }
        }


        private enum OrderStatus
        {
            Initial,
            Buy,
            Sell,
        }

        private int _penalty = 0;
        private OrderStatus _orderStatus = OrderStatus.Initial;
        private Order _previousOrder;


        protected override Direction GetDirection(int index)
        {
            if (_inputs == null)
            {
              _inputs = new float[_tradingItem.InitData.HistoryWidowLength * _tradingItem.InitData.InputData.Indicators.IndicatorNumber + (TradingItemInitData.IS_RECURRENT ? TradingItemInitData.OUTPUT_NUMBER : 0)];
            }

            for (int i = 0; i < _tradingItem.InitData.InputData.Indicators.IndicatorNumber; i++)
            {
                var indicatorIndex = (int)_tradingItem.GetIndicatorIndex(i);
                var indicatorValues = _learningData.Indicators[indicatorIndex].Values;
                Array.Copy(indicatorValues, index, _inputs, _tradingItem.InitData.HistoryWidowLength * i, _tradingItem.InitData.HistoryWidowLength);
            }

            if (TradingItemInitData.IS_RECURRENT)
            {
                Array.Copy(previuosOutput, 0, _inputs,
                    _tradingItem.InitData.HistoryWidowLength * _tradingItem.InitData.InputData.Indicators.IndicatorNumber, TradingItemInitData.OUTPUT_NUMBER);
            }

            var zigzagValue = _learningData.ZigZags[index].Value;

            var outputs = _neuron.NetworkCompute(_inputs);

            if (TradingItemInitData.IS_RECURRENT)
            {
                Array.Copy(outputs, previuosOutput, TradingItemInitData.OUTPUT_NUMBER);
            }

            var isBuySignal = outputs[0] > 0.5F;
            var isSellSignal = outputs[1] > 0.5F;
            var isHoldSignal = outputs[2] > 0.5F;


            var isHold = !isBuySignal && !isSellSignal && isHoldSignal;
            var isBuy = isBuySignal && !isSellSignal && !isHoldSignal;
            var isSell = !isBuySignal && isSellSignal && !isHoldSignal;
            var isClose = !(isHold || isBuy || isSell);


            switch (_orderStatus)
            {
                case OrderStatus.Initial:
                    _penalty = 0;
                    if (isBuy) _orderStatus = OrderStatus.Buy;
                    if (isSell) _orderStatus = OrderStatus.Sell;
                    break;
                case OrderStatus.Buy:
                    if (isSell)
                    {
                        _previousOrder = _currentOrder;
                        _penalty = 0;
                        _orderStatus = OrderStatus.Sell;
                    }
                    if (isClose)
                    {
                        if (_currentOrder != null)
                            _previousOrder = _currentOrder;
                    }
                    if (isBuy || isHold)
                    {
                        switch (zigzagValue)
                        {
                            case 1:
                                if (_previousOrder != null && _previousOrder.Type == OrderType.Buy)
                                {
                                    ++_penalty;
                                    _previousOrder = null;
                                }
                                break;
                            case -1:
                                break;
                            case 0:
                                ++_penalty;
                                break;
                        }

                    }

                    break;

                case OrderStatus.Sell:
                    if (isBuy)
                    {
                        _previousOrder = _currentOrder;
                        _penalty = 0;
                        _orderStatus = OrderStatus.Buy;
                    }
                    if (isClose)
                    {
                        if (_currentOrder != null)
                            _previousOrder = _currentOrder;
                    }
                    if (isSell || isHold)
                    {
                        switch (zigzagValue)
                        {
                            case -1:
                                if (_previousOrder != null && _previousOrder.Type == OrderType.Sell)
                                {
                                    ++_penalty;
                                    _previousOrder = null;
                                }
                                break;
                            case 1:
                                break;
                            case 0:
                                ++_penalty;
                                break;
                        }

                    }
                    break;
            }
            return DefaultSignalsProcess(isHold, isSell, zigzagValue, isBuy);
        }

        private Direction DefaultSignalsProcess(bool isHold, bool isSell, int zigzagValue, bool isBuy)
        {
            if (isHold)
            {
                if (_penalty > 0)
                    _zigZagFitness -= _penalty;
                if (_currentOrder != null)
                {
                    if (_penalty == 0)
                        ++_zigZagFitness;
                }
                else
                {
                    --_zigZagFitness;
                }
                return Direction.Hold;
            }

            if (isSell)
            {
                if (_penalty > 0)
                    _zigZagFitness -= _penalty;
                if (zigzagValue == -1)
                {
                    if (_penalty == 0)
                        ++_zigZagFitness;
                }
                else
                    --_zigZagFitness;

                return Direction.Down;
            }

            if (isBuy)
            {
                if (_penalty > 0)
                    _zigZagFitness -= _penalty;
                if (zigzagValue == 1)
                {
                    if (_penalty == 0)
                        ++_zigZagFitness;
                }
                else
                    --_zigZagFitness;
                return Direction.Up;
            }

            if (zigzagValue == 0)
                ++_zigZagFitness;
            else
                --_zigZagFitness;

            return Direction.Close;
        }

        private void SetDefaultBuyZigZagFitness(int zigzagValue)
        {
            if (zigzagValue == 1)
                ++_zigZagFitness;
            else
                --_zigZagFitness;
        }


        protected override decimal GetStopLoss()
        {
            return (decimal)_tradingItem.StopLoss;
        }
    }
}
