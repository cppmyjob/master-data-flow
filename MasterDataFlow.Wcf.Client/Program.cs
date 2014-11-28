using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MasterDataFlow.Client;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;

namespace MasterDataFlow.Wcf.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var clientContext = new WcfClientContext(new ServiceKey(new Guid(ConfigurationManager.AppSettings["ServerGateKey"]))))
            {
                var runner = new CommandRunner();
                var clientGate = new ClientGate(clientContext);
                runner.ConnectHub(clientGate);
                var сommandWorkflow = new CommandWorkflow();
                runner.ConnectHub(сommandWorkflow);

                сommandWorkflow.MessageRecieved += (key, message) =>
                {
                    Console.WriteLine("Message recieved");
                };

                var dataObject = new MathCommand.MathCommandDataObject()
                {
                    Expression = "33 + 44"
                };
                сommandWorkflow.Start<MathCommand>(dataObject);
                Console.ReadLine();
            }
        }
    }
}
