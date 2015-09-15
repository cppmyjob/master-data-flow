using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Messages;

namespace Examples.Intelligence.MultiGenetic
{

    [Serializable]
    public class TravelingSalesmanProblemGeneticItem : GeneticItem
    {
        public TravelingSalesmanProblemGeneticItem(GeneticInitData initData) : base(initData)
        {
        }

        public override double CreateValue(double random)
        {
            return Math.Floor(random * InitData.Count);
        }
    }

    [Serializable]
    public class TravalingPoint
    {
        public TravalingPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
        public double X { get; set; }
        public double Y { get; set; }
    }

    [Serializable]
    public class TravelingSalesmanProblemInitData : GeneticCellDataObject
    {
        public TravalingPoint[] Points { get; set; }
    }

    public class TravelingSalesmanProblemCommand : GeneticCellCommand<TravelingSalesmanProblemInitData>
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
            return new TravelingSalesmanProblemGeneticItem(initData);
        }

        public override double CalculateFitness(GeneticItem item, int processor)
        {
            var fitness = 0.0;
            bool[] oldValues = new bool[item.Values.Length];
            for (int i = 0; i < item.Values.Length; i++)
            {
                var index = (int) item.Values[i];
                if (oldValues[index])
                    return 0.0;
                oldValues[index] = true;
            }

            for (int i = 1; i < item.Values.Length; i++)
            {
                var point1 = DataObject.Points[(int) item.Values[i - 1]];
                var point2 = DataObject.Points[(int)item.Values[i]];
                var d = Math.Sqrt((point2.X - point1.X)*(point2.X - point1.X) + (point2.Y - point1.Y)*(point2.Y - point1.Y));
                fitness += d;
            }
            if (fitness == 0.0)
                return 0.0;
            else
                return 1 / fitness;
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

        protected override void FillValues(GeneticItem item)
        {
            double[] values = item.Values;
            for (int i = 0; i < item.Values.Length; i++)
            {
                values[i] = i;
            }

            for (int i = values.Length; i > 0; i--)
            {
                int j = Random.Next(i);
                int k = (int)values[j];
                values[j] = values[i - 1];
                values[i - 1] = k;
            }
        }


        //protected override GeneticItem CreateChild(GeneticItem firstParent, GeneticItem secondParent)
        //{
        //    GeneticItem child = InternalCreateItem();
        //    double[] childValues = child.Values;
        //    bool[] isAdded = new bool[DataObject.CellInitData.ValuesCount];
        //    bool[] isNotFreePlace = new bool[DataObject.CellInitData.ValuesCount];

        //    bool isFirst = Random.Next(10) < 5;
        //    double[] firstValues = isFirst ? firstParent.Values : secondParent.Values;
        //    double[] secondValues = isFirst ? secondParent.Values : firstParent.Values;

        //    for (int i = 0; i < DataObject.CellInitData.ValuesCount; i++)
        //    {
        //        int currentValue = (int)(i % 2 == 0 ? firstValues[i] : secondValues[i]);
        //        if (Random.Next(1000) > 950)
        //        {
        //            currentValue = Random.Next(DataObject.CellInitData.ValuesCount);
        //        }
        //        if (!isAdded[currentValue])
        //        {
        //            isAdded[currentValue] = true;
        //            isNotFreePlace[i] = true;
        //            childValues[i] = currentValue;
        //        }
        //    }
        //    var leftArray = new List<int>();
        //    for (int i = 0; i < isNotFreePlace.Length; i++)
        //    {
        //        if (!isAdded[i])
        //            leftArray.Add(i);
        //    }
        //    for (int i = 0; i < isNotFreePlace.Length; i++)
        //    {
        //        if (!isNotFreePlace[i])
        //        {
        //            // TODO Random selection - leftArray[leftArray.Count - 1]
        //            childValues[i] = leftArray[leftArray.Count - 1];
        //            leftArray.RemoveAt(leftArray.Count - 1);
        //        }
        //    }
        //    return child;
        //}
    }
}
