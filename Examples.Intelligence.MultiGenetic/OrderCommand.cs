using System;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Messages;

namespace Examples.Intelligence.MultiGenetic
{
    [Serializable]
    public class OrderGeneticItem : GeneticItem
    {
        public OrderGeneticItem(GeneticInitData initData) : base(initData)
        {
        }

        public override double CreateValue(double random)
        {
            return Math.Floor(random * InitData.Count);
        }
    }

    [Serializable]
    public class OrderDataObject : GeneticCellDataObject
    {
    }


    public class OrderCommand : GeneticCellCommand<GeneticCellDataObject>
    {

        protected override BaseMessage BaseExecute()
        {
            Console.WriteLine("Started Command Key={0}", Key);
            var result = base.BaseExecute();
            Console.WriteLine("Finished Command Key={0}", Key);
            return result;
        }

        protected override GeneticItem CreateItem(GeneticInitData initData)
        {
            return new OrderGeneticItem(initData);
        }

        public override double CalculateFitness(GeneticItem item, int processor)
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

            for (int i = 0; i < item.Values.Length; i++)
            {
                if (i == item.Values[i])
                {
                    result += 1;
                }
            }
            return result;
        }

        //protected override GeneticItem CreateChild(GeneticItem firstParent, GeneticItem secondParent)
        //{
        //    GeneticItem child = InternalCreateItem();
        //    double[] firstValues = firstParent.Values;
        //    double[] secondValues = secondParent.Values;
        //    double[] childValues = child.Values;

        //    for (int i = 0; i < firstValues.Length; i++)
        //    {
        //        if (Random.NextDouble() > 0.8)
        //        {
        //            childValues[i] = child.CreateValue(Random.NextDouble());
        //        }
        //        else
        //        {
        //            if (i % 2 == 0)
        //                childValues[i] = firstValues[i];
        //            else
        //                childValues[i] = secondValues[i];
        //        }
        //    }
        //    return child;
        //}
    }
}
