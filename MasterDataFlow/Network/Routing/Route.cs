using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Network.Routing
{
    public class Route
    {
        private readonly BaseKey _destination;
        private readonly BaseKey _gate;
        private uint _lenght;

        public Route(BaseKey destination, BaseKey gate, uint lenght)
        {
            _destination = destination;
            _gate = gate;
            LastUsage = DateTime.Now;
            _lenght = lenght;
        }

        public void IncrementLenght()
        {
            _lenght++;
        }

        public BaseKey Destination
        {
            get { return _destination;  }
        }

        public BaseKey Gate
        {
            get { return _gate; }
        }

        public DateTime LastUsage { get; set; }

        public uint Lenght
        {
            get { return _lenght;  }
        }
    }
}
