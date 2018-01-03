using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Messages;
using MasterDataFlow.Network;

namespace Examples.Intelligence.MultiGenetic
{
    public class VariableCrossoverController
    {
        public void Execute(CommandWorkflow commandWorkflow)
        {
            using (var @event = new ManualResetEvent(false))
            {
                var dataObject = CreateTravelingSalesmanProblemInitData();
                var instancesCount = 10;

                var theBests = new List<VariableCrossoverGeneticItem>();
                var completedInstances = 0;
                commandWorkflow.MessageRecieved += (key, message) =>
                                                   {
                                                       Console.WriteLine("Response from {0}", key);
                                                       var stopMessage = message as StopCommandMessage;
                                                       if (stopMessage != null)
                                                       {
                                                           var best = (VariableCrossoverGeneticItem)(stopMessage.Data as GeneticInfoDataObject).Best;
                                                           lock (this)
                                                           {
                                                               theBests.Add(best);
                                                               ++completedInstances;
                                                               if (completedInstances == instancesCount)
                                                               {
                                                                   @event.Set();
                                                               }
                                                           }
                                                       }
                                                   };

                for (int i = 0; i < instancesCount; i++)
                {
                    commandWorkflow.Start<VariableCrossoverGeneticCommand>(dataObject);
                }
                @event.WaitOne(1000000);
                Console.WriteLine("The bests");
                foreach (var theBestItem in theBests)
                {
                    PrintTheBest(theBestItem);
                }
                Console.WriteLine("The best fitness is {0}", theBests.Max(t => t.Fitness));

                dataObject = CreateTravelingSalesmanProblemInitData();
                //dataObject.RepeatCount = 200;
                dataObject.InitPopulation = new List<int[]>();
                dataObject.InitLengths = new List<int[]>();
                for (int i = 0; i < theBests.Count; i++)
                {
                    dataObject.InitPopulation.Add(theBests[i].Values);
                    dataObject.InitLengths.Add(theBests[i].Lengths);
                }
                @event.Reset();
                theBests.Clear();
                instancesCount = 1;
                completedInstances = 0;
                commandWorkflow.Start<VariableCrossoverGeneticCommand>(dataObject);
                @event.WaitOne(1000000);
                Console.WriteLine("The bests");
                foreach (var theBestItem in theBests)
                {
                    PrintTheBest(theBestItem);
                }
            }

        }

        private static VariableCrossoverGeneticInitData CreateTravelingSalesmanProblemInitData()
        {
            var initItemData = new GeneticItemInitData(30);
            var initData = new GeneticCommandInitData(1000, 700, 20000);
            var dataObject = new VariableCrossoverGeneticInitData
            {
                                 ItemInitData = initItemData,
                                 CommandInitData = initData,
                                 Points = new[]
                                          {
                                              new TravalingPoint(1, 1), new TravalingPoint(10, 1),
                                              new TravalingPoint(44, 11), new TravalingPoint(4, 4),
                                              new TravalingPoint(8, 8), new TravalingPoint(6, 6),
                                              new TravalingPoint(98, 1), new TravalingPoint(32, 33),
                                              new TravalingPoint(39, 39), new TravalingPoint(45, 02),

                                              new TravalingPoint(4, 1), new TravalingPoint(10, 1),
                                              new TravalingPoint(74, 11), new TravalingPoint(9, 4),
                                              new TravalingPoint(89, 8), new TravalingPoint(2, 6),
                                              new TravalingPoint(98, 51), new TravalingPoint(72, 33),
                                              new TravalingPoint(39, 339), new TravalingPoint(55, 2),

                                              new TravalingPoint(6, 10), new TravalingPoint(101, 13),
                                              new TravalingPoint(74, 111), new TravalingPoint(42, 44),
                                              new TravalingPoint(4, 82), new TravalingPoint(61, 64),
                                              new TravalingPoint(78, 13), new TravalingPoint(321, 334),
                                              new TravalingPoint(39, 394), new TravalingPoint(452, 024),
                                          }
            };
            return dataObject;
        }

        private void PrintTheBest(GeneticItem<GeneticItemInitData, int> theBestItem)
        {
            var values = theBestItem.Values.Select(t => t.ToString(CultureInfo.InvariantCulture)).ToArray();
            var result = string.Join(",", values);
            Console.WriteLine("Fitness={0} values={1}", theBestItem.Fitness, result);
        }
    }

    [Serializable]
    public class VariableCrossoverGeneticItem : GeneticIntItem
    {
        public VariableCrossoverGeneticItem(GeneticItemInitData initData) : base(initData)
        {
            _values = Enumerable.Repeat<int>(-1, initData.ValuesNumber).ToArray();
            Lengths = new int[initData.ValuesNumber];
        }

        public override int CreateValue(IRandom random)
        {
            return (int)(random.NextDouble() * InitData.ValuesNumber);
        }

        public int[] Lengths { get; set; }
    }

    [Serializable]
    public class VariableCrossoverGeneticInitData : GeneticIntDataObject<VariableCrossoverGeneticItem>
    {
        public TravalingPoint[] Points { get; set; }
        public IList<int[]> InitLengths { get; set; }

        public override void Init(VariableCrossoverGeneticItem item, int index)
        {
            base.Init(item, index);
            Array.Copy(InitLengths[index], item.Lengths, InitPopulation[index].Length);
        }
    }

    public class VariableCrossoverGeneticCommand : GeneticIntCommand<VariableCrossoverGeneticInitData, VariableCrossoverGeneticItem>
    {

        protected override BaseMessage BaseExecute()
        {
            Console.WriteLine("Started Command Key={0}", Key);
            var result = base.BaseExecute();
            Console.WriteLine("Finished Command Key={0}", Key);
            return result;
        }

        protected override VariableCrossoverGeneticItem CreateItem(GeneticItemInitData initData)
        {
            return new VariableCrossoverGeneticItem(initData);
        }

        public override double CalculateFitness(VariableCrossoverGeneticItem item, int processor)
        {
            var fitness = 0.0;
            //bool[] oldValues = new bool[item.Values.Length];
            //for (int i = 0; i < item.Values.Length; i++)
            //{
            //    var index = (int)item.Values[i];
            //    if (oldValues[index])
            //        return 0.0;
            //    oldValues[index] = true;
            //}

            for (int i = 1; i < item.Values.Length; i++)
            {
                var point1 = DataObject.Points[(int)item.Values[i - 1]];
                var point2 = DataObject.Points[(int)item.Values[i]];
                var d = Math.Sqrt((point2.X - point1.X) * (point2.X - point1.X) + (point2.Y - point1.Y) * (point2.Y - point1.Y));
                fitness += d;
            }
            if (fitness == 0.0)
                return 0.0;
            else
                return 1 / fitness;
        }

        protected override void Mutation(VariableCrossoverGeneticItem item)
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
        }


        protected override VariableCrossoverGeneticItem CreateChild(VariableCrossoverGeneticItem firstParent,
            VariableCrossoverGeneticItem secondParent)
        {
            if (firstParent == secondParent)
            {
                return null;
            }
            var child = InternalCreateItem();
            int[] firstValues = firstParent.Values;
            int[] secondValues = secondParent.Values;
            int[] childValues = child.Values;

            int index = 0;
            while (index < child.Values.Length)
            {
                var item = Random.NextDouble() > 0.5 ? firstParent : secondParent;
                //var item = index % 2 == 0 ? firstParent : secondParent;

                var l = item.Lengths[index];
                if (index + l > child.Values.Length)
                {
                    l = child.Values.Length - index;
                }
                Array.Copy(item.Values, index, childValues, index, l);
                Array.Copy(item.Lengths, index, child.Lengths, index, l);
                index += l;
            }

            var values = new bool[child.Values.Length];
            var indexes = new bool[child.Values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                if (childValues[i] < 0) continue;
                if (indexes[childValues[i]])
                {
                    values[i] = false;
                }
                else
                {
                    indexes[childValues[i]] = true;
                    values[i] = true;
                }
            }
            var j = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (!values[i])
                {
                    while (indexes[j])
                    {
                        ++j;
                    }
                    childValues[i] = j++;
                }
            }

            return child;
        }

        protected override void FillValues(VariableCrossoverGeneticItem item)
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
            AddLengths(item);
        }

        private void AddLengths(VariableCrossoverGeneticItem item)
        {
            for (int i = 0; i < DataObject.ItemInitData.ValuesNumber; i++)
            {
                var length = Random.Next(DataObject.ItemInitData.ValuesNumber) / 3 + 1;
                //var length = 3;
                if (i + length > DataObject.ItemInitData.ValuesNumber)
                {
                    length = DataObject.ItemInitData.ValuesNumber - i;
                }
                item.Lengths[i] = length;
            }
        }
    }

}
