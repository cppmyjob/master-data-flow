using System;
using System.Configuration;
using MasterDataFlow.Client;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Network;

namespace Examples.Wcf.Client
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
                    var stopMessage = (StopCommandMessage)message;
                    var result = (MathCommand.MathCommandResult)stopMessage.Data;
                    Console.WriteLine("Result is {0}", result.Result);
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
