using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Messages;

namespace MasterDataFlow.Common.Tests.Genetic
{
    [Serializable]
    public class OrderGeneticItem : GeneticDoubleItem
    {
        public OrderGeneticItem(GeneticItemInitData initData) : base(initData)
        {
        }

        public override double CreateValue(IRandom random)
        {
            return Math.Floor(random.NextDouble() * InitData.Count);
        }
    }

    [Serializable]
    public class OrderDataObject : GeneticDoubleDataObject
    {
    }

    public class OrderCommand : GeneticDoubleCommand<OrderDataObject>
    {
        public static GeneticDataObject<double> StaticDataObject;

        protected override BaseMessage BaseExecute()
        {
            StaticDataObject = DataObject;
            //Console.WriteLine("OrderCommand::BaseExecute");
            return base.BaseExecute();
        }

        protected override GeneticDoubleItem CreateItem(GeneticItemInitData initData)
        {
            return new OrderGeneticItem(initData);
        }

        public override double CalculateFitness(GeneticDoubleItem item, int processor)
        {
            var result = 1;
            var lastValue = item.Values[0];
            for (int i = 1; i < item.Values.Length; i++)
            {
                if (lastValue < item.Values[i])
                {
                    lastValue = item.Values[i];
                    result += 1;
                }
                else
                    break;
            }

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < item.Values.Length; j++)
                {
                    if (i == item.Values[j])
                    {
                        result += 1;
                        break;
                    }
                }

            }
            return result;
        }
    }
}
