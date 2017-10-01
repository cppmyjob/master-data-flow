using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Test;
using MasterDataFlow.Trading.Common;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Genetic
{

    public class FNDirectionTester : FxDirectionTester
    {
        private const double START_DEPOSIT = 1000;

        private GeneticNeuronDLL1 _dll;
        private double[] _avg1;
        private double[] _avg2;
        private double[] _avg3;

        private FNGeneticItem _item;
        private Bar[] _bars;

        public FNDirectionTester(GeneticNeuronDLL1 dll, Bar[] bars, FNGeneticItem item, int fromBar, int length, double[] avg1, double[] avg2, double[] avg3) :
            base(START_DEPOSIT, bars, fromBar, length)
        {
            _bars = bars;
            _item = item;
            _dll = dll;
            _avg1 = avg1;
            _avg2 = avg2;
            _avg3 = avg3;
        }

        protected override FxDirectionGetDirection GetDirectionDelegate()
        {
            return DirectionGetDirection;
        }

        private double[] _inputs = new double[FNGeneticManager.BAR_COUNT * FNGeneticManager.AVG_COUNT + FNGeneticManager.ADD_PARAMETERS];

        private FxDirection DirectionGetDirection(int index)
        {
            Array.Copy(_avg1, index - FNGeneticManager.BAR_COUNT, _inputs, FNGeneticManager.BAR_COUNT * 0, FNGeneticManager.BAR_COUNT);
            Array.Copy(_avg2, index - FNGeneticManager.BAR_COUNT, _inputs, FNGeneticManager.BAR_COUNT * 1, FNGeneticManager.BAR_COUNT);
            Array.Copy(_avg3, index - FNGeneticManager.BAR_COUNT, _inputs, FNGeneticManager.BAR_COUNT * 2, FNGeneticManager.BAR_COUNT);

            int addValueOffset = FNGeneticManager.BAR_COUNT * FNGeneticManager.AVG_COUNT;
            _inputs[addValueOffset] = GetAddValueOrder();
            _inputs[addValueOffset + 1] = GetAddValueEquity();
            _inputs[addValueOffset + 2] = GetAddValueLastOrderResult();
            _inputs[addValueOffset + 3] = GetAddValueLastOrderPeriod();
            _inputs[addValueOffset + 4] = GetAddValueOrderOpenPeriod();
            _inputs[addValueOffset + 5] = GetAddValueHistoryOrder();

            double[] outputs = _dll.NetworkCompute(_inputs);
            bool isBuy = outputs[0] > 0.5;
            bool isSell = outputs[1] > 0.5;
            if (isBuy && isSell)
                return FxDirection.None;
            if (isBuy)
                return FxDirection.Up;
            if (isSell)
                return FxDirection.Down;
            return FxDirection.None;
        }


        public static void SaveInputsToFile(double[] inputs, double[] outputs)
        {
            using (StreamWriter writer = new StreamWriter("testdata.csv", true))
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    writer.WriteLine("Outputs[" + i.ToString() + "] = " + inputs[i].ToString("F10"));
                }

                writer.WriteLine();

                for (int i = 0; i < outputs.Length; i++)
                {
                    writer.WriteLine("result[" + i.ToString() + "] = " + outputs[i].ToString("F10"));
                }


            }
        }

        private double GetAddValueOrderOpenPeriod()
        {
            if (Orders.Count == 0)
                return 0;

            int diff = CurrentBar - Orders.First().Value.OpenBarIndex;
            double result = (double)diff * (double)Point;
            if (result > 1.0)
                return 1.0;
            else
                return result;


        }

        private double GetAddValueLastOrderPeriod()
        {
            if (Orders.Count == 0)
            {
                if (History.Count == 0)
                    return 0;
                int diff = CurrentBar - History[History.Count - 1].Order.CloseBarIndex;
                double result = (double)diff * (double)Point;
                if (result > 1.0)
                    return 1.0;
                else
                    return result;
            }
            else
                return 0;
        }

        private double GetAddValueLastOrderResult()
        {
            if (Orders.Count == 0)
            {
                if (History.Count == 0)
                    return 0;
                if (History[History.Count - 1].Profit >= 0)
                    return 1;
                else
                    return 0.5;
            }
            else
                return 0;
        }

        private double GetAddValueEquity()
        {
            double value = GetCurrentEquity();
            if (value > 1.0)
                value = 1.0;
            if (value < -1.0)
                value = -1.0;
            return (value + 1.0) / 2;
        }

        private double GetAddValueOrder()
        {
            if (Orders.Count == 0)
                return 0.5;
            return Orders.First().Value.Type == FxOrderType.Sell ? 0 : 1;
        }

        private double GetAddValueHistoryOrder()
        {

            if (Orders.Count == 0)
            {
                if (History.Count == 0)
                    return 0.5;
                return History[History.Count - 1].Order.Type == FxOrderType.Sell ? 0 : 1;
            }
            else
                return 0.5;
        }


        protected override int GetStopLoss()
        {
            return _item.StopLoss;
        }

    }

}
