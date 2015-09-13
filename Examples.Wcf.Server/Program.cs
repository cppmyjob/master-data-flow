using System;
using System.ServiceModel;
using MasterDataFlow.Keys;
using MasterDataFlow.Server;
using MasterDataFlow.Utils;

namespace Examples.Wcf.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var ck = new CommandKey();
            var sk = new ServiceKey();
            var wk = new WorkflowKey();

            Logger.SetFactory(new ConsoleLoggerOutputFactory());

            using (var serviceHost = new ServiceHost(typeof(WcfServer)))
            {
                serviceHost.Open();

                Console.WriteLine("The service is ready. Click Enter to exit");
                Console.ReadLine();
            }
        }
    }
}
