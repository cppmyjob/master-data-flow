using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Messages;
using MasterDataFlow.Network;

namespace Examples.Intelligence.MultiGenetic
{
    public class MultiGeneticController
    {
        //public void Execute(CommandWorkflow commandWorkflow)
        //{
        //    using (var @event = new ManualResetEvent(false))
        //    {
        //        var initData = new GeneticCellInitData(1000, 300, 50);
        //        var dataObject = new OrderDataObject
        //                         {
        //                             CellInitData = initData,
        //                             RepeatCount = 500
        //                         };
        //        var instancesCount = 4;

        //        List<GeneticItem> theBests = new List<GeneticItem>();
        //        var completedInstances = 0;
        //        commandWorkflow.MessageRecieved += (key, message) =>
        //                                           {
        //                                               Console.WriteLine("Response from {0}", key);
        //                                               var stopMessage = message as StopCommandMessage;
        //                                               if (stopMessage != null)
        //                                               {
        //                                                   var best = (GeneticItem) (stopMessage.Data as GeneticStopDataObject).Best;
        //                                                   lock (this)
        //                                                   {
        //                                                       theBests.Add(best);
        //                                                       ++completedInstances;
        //                                                       if (completedInstances == instancesCount)
        //                                                       {
        //                                                           @event.Set();
        //                                                       }
        //                                                   }
        //                                               }
        //                                           };

        //        for (int i = 0; i < instancesCount; i++)
        //        {
        //            commandWorkflow.Start<OrderCommand>(dataObject);
        //        }
        //        @event.WaitOne(1000000);
        //        Console.WriteLine("The bests");
        //        foreach (var theBestItem in theBests)
        //        {
        //            PrintTheBest(theBestItem);
        //        }

        //    }

        //}


        public void Execute(CommandWorkflow commandWorkflow)
        {
            using (var @event = new ManualResetEvent(false))
            {
                var dataObject = CreateTravelingSalesmanProblemInitData();
                var instancesCount = 7;

                var theBests = new List<TravelingSalesmanProblemGeneticItem>();
                var completedInstances = 0;
                commandWorkflow.MessageRecieved += (key, message) =>
                                                   {
                                                       Console.WriteLine("Response from {0}", key);
                                                       var stopMessage = message as StopCommandMessage;
                                                       if (stopMessage != null)
                                                       {
                                                           var best = (TravelingSalesmanProblemGeneticItem)(stopMessage.Data as GeneticInfoDataObject).Best;
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
                    commandWorkflow.Start<TravelingSalesmanProblemCommand>(dataObject);
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
                for (int i = 0; i < theBests.Count; i++)
                {
                    dataObject.InitPopulation.Add(theBests[i].Values);
                }
                @event.Reset();
                theBests.Clear();
                instancesCount = 1;
                completedInstances = 0;
                commandWorkflow.Start<TravelingSalesmanProblemCommand>(dataObject);
                @event.WaitOne(1000000);
                Console.WriteLine("The bests");
                foreach (var theBestItem in theBests)
                {
                    PrintTheBest(theBestItem);
                }
            }

        }

        private static TravelingSalesmanProblemInitData CreateTravelingSalesmanProblemInitData()
        {
            var initItemData = new GeneticItemInitData(20);
            var initData = new GeneticCommandInitData(1000, 330, 10000);
            var dataObject = new TravelingSalesmanProblemInitData
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

                                          }
            };
            return dataObject;
        }

        private void PrintTheBest(TravelingSalesmanProblemGeneticItem theBestItem)
        {
            var values = theBestItem.Values.Select(t => t.ToString(CultureInfo.InvariantCulture)).ToArray();
            var result = string.Join(",", values);
            Console.WriteLine("Fitness={0} values={1}", theBestItem.Fitness, result);
        }
    }
}
