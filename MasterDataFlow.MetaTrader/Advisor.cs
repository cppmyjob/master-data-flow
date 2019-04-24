using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using RGiesecke.DllExport;

namespace MasterDataFlow.MetaTrader
{
    public static class Advisor
    {
        [DllExport("AdvisorInit", CallingConvention = CallingConvention.StdCall)]
        public static int AdvisorInit([In, Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder response)
        {
            response[0] = 't';
            response[1] = 'r';
            response[2] = 'u';
            response[3] = 'e';
            Log(Directory.GetCurrentDirectory());
            return 0;
        }

        [DllExport("AdvisorTick", CallingConvention = CallingConvention.StdCall)]
        public static int AdvisorTick([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] tab, int size)
        {
            return tab[0];
        }

        private static void Log(string message)
        {
            var date = DateTime.Now.ToString("s");
            File.AppendAllLines(@"c:\tmp\masterdata.log", new []{ date + " " + message });
        }

    }
}
