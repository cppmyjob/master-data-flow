﻿using System;
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

        private int _zigZagSellCount = 0;
        private int _zigZagValidSellCount = 0;

        private int _zigZagBuyCount = 0;
        private int _zigZagValidBuyCount = 0;

        public DirectionTester(GeneticNeuronDLL1 dll, TradingItem tradingItem, LearningData learningData) 
            : base(START_DEPOSIT, learningData.Prices, 0, learningData.Prices.Length)
        {
            _dll = dll;
            _tradingItem = tradingItem;
            _learningData = learningData;
        }

        private float[] _inputs;

        public double ZigZagCount
        {
            get { 
                if (_zigZagValidBuyCount <= 0 || _zigZagValidSellCount <= 0)
                      return Double.MinValue;
                var br = (double) _zigZagValidBuyCount / _zigZagBuyCount;
                var sr = (double) _zigZagValidSellCount / _zigZagSellCount; ;
                //if (br > sr && br / sr > 2)
                //    return Double.MinValue + 1;
                //if (sr > br && sr / br > 2)
                //    return Double.MinValue + 1;

                //var avg = (_zigZagBuyCount + _zigZagSellCount) / 2;

                //var otkl = Math.Sqrt((_zigZagValidBuyCount - avg) * (_zigZagValidBuyCount - avg) +
                //                     (_zigZagValidSellCount - avg) * (_zigZagValidSellCount - avg));

                //return 1 / otkl; 

                return br * sr;
            }
        }

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

            var zigzagValue = _learningData.ZigZags[index].Value;
            if (zigzagValue == 1)
                ++_zigZagBuyCount;
            if (zigzagValue == -1)
                ++_zigZagSellCount;

            var outputs = _dll.NetworkCompute(_inputs);
            var isBuy = outputs[0] > 0.5F;
            var isSell = outputs[1] > 0.5F;
            if (isBuy && isSell)
            {
                return Direction.None;
            }

            if (isBuy)
            {
                if (zigzagValue == 1)
                {
                    ++_zigZagValidBuyCount;
                }

                return Direction.Up;
            }

            if (isSell)
            {
                if (zigzagValue == -1)
                {
                    ++_zigZagValidSellCount;
                }

                return Direction.Down;
            }
            return Direction.None;
        }


        protected override decimal GetStopLoss()
        {
            return (decimal)_tradingItem.StopLoss;
        }
    }
}
