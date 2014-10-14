using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Network
{
    public abstract class Gate : EventLoopHub
    {
        private readonly ServiceKey _key = new ServiceKey();

        public override BaseKey Key
        {
            get { return _key; }
        }

    }
}
