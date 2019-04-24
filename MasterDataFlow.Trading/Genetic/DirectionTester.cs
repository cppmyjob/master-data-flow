using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Intelligence.Neuron;
using MasterDataFlow.Intelligence.Neuron.SimpleNeuron;
using MasterDataFlow.Trading.Algorithms;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Genetic
{
    public class DirectionTester : Tester.DirectionTester
    {
        private readonly ISimpleNeuron _neuron;
        private readonly TradingItem _tradingItem;
        private readonly LearningData _learningData;
        private readonly NeuralNetworkAlgorithm _algorithm;
        private const decimal START_DEPOSIT = 100000;

        private int _zigZagFitness = 0;

        private float[] previuosOutput = new float[TradingItemInitData.OUTPUT_NUMBER];

        public DirectionTester(ISimpleNeuron neuron, TradingItem tradingItem, LearningData learningData) 
            : base(START_DEPOSIT, learningData.Prices, 0, learningData.Prices.Length)
        {
            _neuron = neuron;
            _tradingItem = tradingItem;
            _learningData = learningData;

            _algorithm = new NeuralNetworkAlgorithm(tradingItem, neuron);
        }

        private float[] _inputs;

        public int ZigZagCount
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
                // Получение номера индикатора из tradingItem
                var indicatorIndex = (int)_tradingItem.GetIndicatorIndex(i);
                var indicatorValues = _learningData.Indicators[indicatorIndex].Values;
                // копирование в _inputs данных индикаторов
                Array.Copy(indicatorValues, index, _inputs, _tradingItem.InitData.HistoryWidowLength * i, _tradingItem.InitData.HistoryWidowLength);
            }

            if (TradingItemInitData.IS_RECURRENT)
            {
                Array.Copy(previuosOutput, 0, _inputs,
                    _tradingItem.InitData.HistoryWidowLength * _tradingItem.InitData.InputData.Indicators.IndicatorNumber, TradingItemInitData.OUTPUT_NUMBER);
            }

            var zigzagValue = _learningData.ZigZags[index].Value;


            var signal = _algorithm.GetSignal(_inputs, previuosOutput);


            switch (_orderStatus)
            {
                case OrderStatus.Initial:
                    _penalty = 0;
                    if (signal == AlgorithmSignal.Buy) _orderStatus = OrderStatus.Buy;
                    if (signal == AlgorithmSignal.Sell) _orderStatus = OrderStatus.Sell;
                    break;
                case OrderStatus.Buy:
                    if (signal == AlgorithmSignal.Sell)
                    {
                        _previousOrder = _currentOrder;
                        _penalty = 0;
                        _orderStatus = OrderStatus.Sell;
                    }
                    if (signal == AlgorithmSignal.Close)
                    {
                        if (_currentOrder != null)
                            _previousOrder = _currentOrder;
                    }
                    if (signal == AlgorithmSignal.Buy || signal == AlgorithmSignal.Hold)
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
                    if (signal == AlgorithmSignal.Buy)
                    {
                        _previousOrder = _currentOrder;
                        _penalty = 0;
                        _orderStatus = OrderStatus.Buy;
                    }
                    if (signal == AlgorithmSignal.Close)
                    {
                        if (_currentOrder != null)
                            _previousOrder = _currentOrder;
                    }
                    if (signal == AlgorithmSignal.Sell || signal == AlgorithmSignal.Hold)
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
            return DefaultSignalsProcess(signal, zigzagValue);
        }

        private Direction DefaultSignalsProcess(AlgorithmSignal signal, int zigzagValue)
        {
            if (signal == AlgorithmSignal.Hold)
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

            if (signal == AlgorithmSignal.Sell)
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

            if (signal == AlgorithmSignal.Buy)
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
