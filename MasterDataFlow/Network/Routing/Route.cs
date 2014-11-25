using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Network.Routing
{
    public class Route
    {
        public Route()
        {
            LastUsage = DateTime.Now;
        }

        public Route(BaseKey destination, BaseKey gate, uint lenght)
        {
            Destination = destination;
            Gate = gate;
            Lenght = lenght;
            LastUsage = DateTime.Now;
        }

        public BaseKey Destination { get; set; }
        public BaseKey Gate { get; set; }
        public uint Lenght { get; set; }

        public void IncrementLenght()
        {
            Lenght++;
        }

        public DateTime LastUsage { get; set; }

    }
}
