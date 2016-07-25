using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Messages;

namespace Examples.Intelligence.MultiGenetic
{

    [Serializable]
    public class TravelingSalesmanProblemGeneticItem : GeneticIntItem
    {
        public TravelingSalesmanProblemGeneticItem(GeneticItemInitData initData) : base(initData)
        {
        }

        public override int CreateValue(IRandom random)
        {
            return (int)(random.NextDouble() * InitData.Count);
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
    public class TravelingSalesmanProblemInitData : GeneticIntDataObject
    {
        public TravalingPoint[] Points { get; set; }
    }

    public class TravelingSalesmanProblemCommand : GeneticIntCommand<TravelingSalesmanProblemInitData>
    {

        protected override BaseMessage BaseExecute()
        {
            Console.WriteLine("Started Command Key={0}", Key);
            var result = base.BaseExecute();
            Console.WriteLine("Finished Command Key={0}", Key);
            return result;
        }

        protected override GeneticIntItem CreateItem(GeneticItemInitData initData)
        {
            return new TravelingSalesmanProblemGeneticItem(initData);
        }

        public override double CalculateFitness(GeneticIntItem item, int processor)
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

        protected override void Mutation(GeneticIntItem item)
        {
            var changeIndex = Random.Next(item.Values.Length);

            var newValue = item.CreateValue(Random);
            var oldValue = item.Values[changeIndex];
            item.Values[changeIndex] = newValue;
            for (int j = 0; j < item.Values.Length; j++)
            {
                if (j != changeIndex && item.Values[j] == newValue)
                {
                    item.Values[j] = oldValue;
                    break;
                }
            }


            //for (int i = 0; i < item.Values.Length; i++)
            //{
            //    if (Random.NextDouble() > 0.9)
            //    {
            //        var newValue = item.CreateValue(Random);
            //        var oldValue = item.Values[i];
            //        item.Values[i] = newValue;
            //        for (int j = 0; j < item.Values.Length; j++)
            //        {
            //            if (j != i && item.Values[j] == newValue)
            //            {
            //                item.Values[j] = oldValue;
            //                break;
            //            }
            //        }
            //    }
            //}
        }


        protected override GeneticIntItem CreateChild(GeneticIntItem firstParent, GeneticIntItem secondParent)
        {
            var child = InternalCreateItem();
            Array.Copy(firstParent.Values, child.Values, firstParent.Values.Length);
            return child;
        }

        protected override void FillValues(GeneticIntItem item)
        {
            var values = item.Values;
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
