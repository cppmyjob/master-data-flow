using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using MasterDataFlow.Assemblies;
using MasterDataFlow.Keys;
using MasterDataFlow.Server;

namespace MasterDataFlow.Wcf.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var ck = new CommandKey();
            var sk = new ServiceKey();
            var wk = new WorkflowKey();

            using (var serviceHost = new ServiceHost(typeof(WcfServer)))
            {
                serviceHost.Open();

                Console.WriteLine("The service is ready. Click Enter to exit");
                Console.ReadLine();
            }
        }
    }
}
