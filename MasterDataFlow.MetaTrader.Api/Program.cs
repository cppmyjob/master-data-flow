using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace MasterDataFlow.MetaTrader.Api
{
    class Program
    {
        static void Main()
        {
            string baseAddress = "http://vcap.me:9000/";

            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.ReadLine();
            }
        }
    }
}
