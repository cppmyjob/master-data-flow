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
    public class TravelingSalesmanProblemMendelGeneticItem : MendelGeneticItem
    {
        public TravelingSalesmanProblemMendelGeneticItem(GeneticItemInitData initData, IRandom random) : base(initData, random)
        {
        }
    }


    [Serializable]
    public class TravelingSalesmanProblemMendelInitData : MendelGeneticDataObject
    {
        public TravalingPoint[] Points { get; set; }
    }

    public class TravelingSalesmanProblemMendelCommand : MendelGeneticCommand<TravelingSalesmanProblemMendelInitData>
    {

        protected override BaseMessage BaseExecute()
        {
            Console.WriteLine("Started Command Key={0}", Key);
            var result = base.BaseExecute();
            Console.WriteLine("Finished Command Key={0}", Key);
            return result;
        }

        protected override MendelGeneticItem CreateItem(GeneticItemInitData initData)
        {
            return new TravelingSalesmanProblemMendelGeneticItem(initData, Random);
        }

        public override double CalculateFitness(MendelGeneticItem item, int processor)
        {
            var fitness = 0.0;
            var oldValues = new bool[item.Values.Length];
            for (var i = 0; i < item.Values.Length; i++)
            {
                var index = item.Values[i].Value;
                if (oldValues[index])
                    return 0.0;
                oldValues[index] = true;
            }

            for (int i = 1; i < item.Values.Length; i++)
            {
                var point1 = DataObject.Points[item.Values[i - 1].Value];
                var point2 = DataObject.Points[item.Values[i].Value];
                var d = Math.Sqrt((point2.X - point1.X) * (point2.X - point1.X) + (point2.Y - point1.Y) * (point2.Y - point1.Y));
                fitness += d;
            }
            if (Math.Abs(fitness) > 0)
                return 1 / fitness;
            else
                return 0.0;
        }

        //protected override MendelGeneticItem CreateChild(MendelGeneticItem firstParent, MendelGeneticItem secondParent)
        //{
        //    var child = InternalCreateItem();
        //    for (int i = 0; i < secondParent.Values.Length; i++)
        //    {
        //        child.Values[i] = secondParent.Values[i];
        //    }
        //    return child;
        //}

        //protected override void FillValues(MendelGeneticItem item)
        //{
        //    var values = item.Values;
        //    for (int i = 0; i < item.Values.Length; i++)
        //    {
        //        var value = item.CreateValue(Random);
        //        value.Alleles[0].Value = i;
        //        value.Alleles[0].IsDominante = true;
        //        //value.Alleles[1].Value = i;
        //        //value.Alleles[1].IsDominante = false;
        //        value.SelectDominancValue(Random);
        //        item.Values[i] = value;
        //    }

        //    //for (int i = values.Length; i > 0; i--)
        //    //{
        //    //    int j = Random.Next(i);
        //    //    var k = values[j];
        //    //    values[j] = values[i - 1];
        //    //    values[i - 1] = k;
        //    //}
        //}

    }
}
