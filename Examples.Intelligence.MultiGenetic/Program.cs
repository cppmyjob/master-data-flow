using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Common.Tests;

namespace Examples.Intelligence.MultiGenetic
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.SetWindowSize(200, 64);

            using (var remote = new RemoteEnvironment())
            {
                var command = new TravelingSalesmanProblemMendelController();
                //var command = new MultiGeneticController();
                command.Execute(remote.CommandWorkflow);
            }
            Console.WriteLine("Finished");
            Console.ReadLine();

        }
    }
}
