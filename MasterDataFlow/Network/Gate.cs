using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Network
{
    public abstract class Gate : ActionHub
    {
        private readonly ServiceKey _key;

        protected Gate()
        {
            _key = new ServiceKey();
        }

        protected Gate(ServiceKey key)
        {
            _key = key;
        }

        public override BaseKey Key
        {
            get { return _key; }
        }

    }
}
