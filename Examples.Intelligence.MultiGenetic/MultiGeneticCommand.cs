﻿using System;
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
    public class MultiGeneticCommand
    {
        public void Execute(CommandWorkflow commandWorkflow)
        {
            using (var @event = new ManualResetEvent(false))
            {
                var initData = new GeneticCellInitData(1000, 300, 50);
                var dataObject = new OrderDataObject
                                 {
                                     CellInitData = initData,
                                     RepeatCount = 500
                                 };
                var instancesCount = 4;

                List<GeneticItem> theBests = new List<GeneticItem>();
                var completedInstances = 0;
                commandWorkflow.MessageRecieved += (key, message) =>
                                                   {
                                                       Console.WriteLine("Response from {0}", key);
                                                       var stopMessage = message as StopCommandMessage;
                                                       if (stopMessage != null)
                                                       {
                                                           var best = (GeneticItem) (stopMessage.Data as GeneticStopDataObject).Best;
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
                    commandWorkflow.Start<OrderCommand>(dataObject);
                }
                @event.WaitOne(1000000);
                Console.WriteLine("The bests");
                foreach (var theBestItem in theBests)
                {
                    PrintTheBest(theBestItem);
                }

            }

        }

        private void PrintTheBest(GeneticItem theBestItem)
        {
            var values = theBestItem.Values.Select(t => t.ToString(CultureInfo.InvariantCulture)).ToArray();
            var result = string.Join(",", values);
            Console.WriteLine("Fitness={0} values={1}", theBestItem.Fitness, result);
        }
    }
}
