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

        public Route(BaseKey destination, BaseKey gate, uint length)
        {
            Destination = destination;
            Gate = gate;
            Length = length;
            LastUsage = DateTime.Now;
        }

        public BaseKey Destination { get; set; }
        public BaseKey Gate { get; set; }
        public uint Length { get; set; }

        public void IncrementLength()
        {
            Length++;
        }

        public DateTime LastUsage { get; set; }

    }
}
