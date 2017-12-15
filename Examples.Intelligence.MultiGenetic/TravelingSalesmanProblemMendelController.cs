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
    public class TravelingSalesmanProblemMendelController
    {
        public void Execute(CommandWorkflow commandWorkflow)
        {
            using (var @event = new ManualResetEvent(false))
            {
                var dataObject = CreateTravelingSalesmanProblemInitData(1000);
                var instancesCount = 7;

                var theBests = new List<MendelGeneticItem>();
                var completedInstances = 0;
                commandWorkflow.MessageRecieved += (key, message) =>
                {
                    Console.WriteLine("Response from {0}", key);
                    var stopMessage = message as StopCommandMessage;
                    if (stopMessage != null)
                    {
                        var best = (MendelGeneticItem)(stopMessage.Data as GeneticInfoDataObject).Best;
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
                    commandWorkflow.Start<TravelingSalesmanProblemMendelCommand>(dataObject);
                }
                @event.WaitOne(1000000);
                Console.WriteLine("The bests");
                foreach (var theBestItem in theBests)
                {
                    PrintTheBest(theBestItem);
                }
                Console.WriteLine("The best fitness is {0}", theBests.Max(t => t.Fitness));

                dataObject = CreateTravelingSalesmanProblemInitData(200);
                dataObject.InitPopulation = new List<GenePair[]>();
                for (int i = 0; i < theBests.Count; i++)
                {
                    dataObject.InitPopulation.Add(theBests[i].Values);
                }
                @event.Reset();
                theBests.Clear();
                instancesCount = 1;
                completedInstances = 0;
                commandWorkflow.Start<TravelingSalesmanProblemMendelCommand>(dataObject);
                @event.WaitOne(1000000);
                Console.WriteLine("The bests");
                foreach (var theBestItem in theBests)
                {
                    PrintTheBest(theBestItem);
                }
            }

        }

        private static TravelingSalesmanProblemMendelInitData CreateTravelingSalesmanProblemInitData(int repeatCount)
        {
            var initItemData = new GeneticItemInitData(10, true);
            var initData = new GeneticCommandInitData(1000, 380, repeatCount);
            var dataObject = new TravelingSalesmanProblemMendelInitData
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
                                          }
            };
            return dataObject;
        }

        private void PrintTheBest(MendelGeneticItem theBestItem)
        {
            var values = theBestItem.Values.Select(t => t.Value.ToString(CultureInfo.InvariantCulture)).ToArray();
            var result = string.Join(",", values);
            Console.WriteLine("Fitness={0} values={1}", theBestItem.Fitness, result);
        }

    }
}
