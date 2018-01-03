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
        public const int OUTPUT_NUMBER = 3;

        private readonly GeneticNeuronDLL1 _dll;
        private readonly TradingItem _tradingItem;
        private readonly LearningData _learningData;
        private const decimal START_DEPOSIT = 100000;

        private int _zigZagSellCount = 0;
        private int _zigZagValidSellCount = 0;

        private int _zigZagBuyCount = 0;
        private int _zigZagValidBuyCount = 0;

        private int _zigZagCloseCount = 0;
        private int _zigZagValidCloseCount = 0;

        private int _zigZagFitness = 0;

        private float[] previuosOutput = new float[OUTPUT_NUMBER];

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
            get
            {

                return _zigZagFitness;
                //if (_zigZagValidBuyCount <= 0 || _zigZagValidSellCount <= 0 || _zigZagValidCloseCount <= 0)
                //      return Double.MinValue;
                //var br = (double) _zigZagValidBuyCount / _zigZagBuyCount;
                //var sr = (double) _zigZagValidSellCount / _zigZagSellCount;
                //var cr = (double)_zigZagValidCloseCount / _zigZagCloseCount;
                //if (br > sr && br / sr > 2)
                //    return Double.MinValue + 1;
                //if (sr > br && sr / br > 2)
                //    return Double.MinValue + 1;

                //var avg = (_zigZagBuyCount + _zigZagSellCount) / 2;

                //var otkl = Math.Sqrt((_zigZagValidBuyCount - avg) * (_zigZagValidBuyCount - avg) +
                //                     (_zigZagValidSellCount - avg) * (_zigZagValidSellCount - avg));

                //return 1 / otkl; 

                //return br * sr * cr;
            }
        }

        protected override Direction GetDirection(int index)
        {
            if (_inputs == null)
            {
              _inputs = new float[_tradingItem.InitData.HistoryWidowLength * _tradingItem.InitData.InputData.Indicators.IndicatorNumber + (TradingItemInitData.IS_RECURRENT ? OUTPUT_NUMBER : 0)];
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
                    _tradingItem.InitData.HistoryWidowLength * _tradingItem.InitData.InputData.Indicators.IndicatorNumber, OUTPUT_NUMBER);
            }

            var zigzagValue = _learningData.ZigZags[index].Value;
            switch (zigzagValue)
            {
                case -1:
                    ++_zigZagSellCount;
                    break;
                case 0:
                    ++_zigZagCloseCount;
                    break;
                case 1:
                    ++_zigZagBuyCount;
                    break;
            }

            var outputs = _dll.NetworkCompute(_inputs);

            if (TradingItemInitData.IS_RECURRENT)
            {
                Array.Copy(outputs, previuosOutput, OUTPUT_NUMBER);
            }

            var isBuy = outputs[0] > 0.5F;
            var isSell = outputs[1] > 0.5F;
            var isHold = outputs[2] > 0.5F;

            if (!isBuy && !isSell && isHold)
            {
                if (_currentOrder != null)
                {
                    ++_zigZagFitness;
                }
                else
                {
                    --_zigZagFitness;
                }
                return Direction.Hold;
            }

            if (!isBuy && isSell && !isHold)
            {
                if (zigzagValue == -1)
                    ++_zigZagFitness;
                else
                    --_zigZagFitness;

                return Direction.Down;
            }

            if (isBuy && !isSell && !isHold)
            {
                if (zigzagValue == 1)
                    ++_zigZagFitness;
                else
                    --_zigZagFitness;
                return Direction.Up;
            }

            //if (isBuy)
            //{
            //    if (zigzagValue == 1)
            //    {
            //        ++_zigZagValidBuyCount;
            //    }

            //    return Direction.Up;
            //}

            //if (isSell)
            //{
            //    if (zigzagValue == -1)
            //    {
            //        ++_zigZagValidSellCount;
            //    }

            //    return Direction.Down;
            //}

            //if (zigzagValue == 0)
            //{
            //    ++_zigZagValidCloseCount;
            //}

            if (zigzagValue == 0)
                ++_zigZagFitness;
            else
                --_zigZagFitness;

            return Direction.Close;
        }


        protected override decimal GetStopLoss()
        {
            return (decimal)_tradingItem.StopLoss;
        }
    }
}
