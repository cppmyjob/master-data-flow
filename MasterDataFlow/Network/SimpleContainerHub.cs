using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Network
{
    public class SimpleContainerHub : Hub
    {
        private ServiceKey _key = new ServiceKey();
        private CommandRunnerHub _runner;

        public override BaseKey Key
        {
            get { return _key; }
        }

        public override void AcceptHub(IHub hub)
        {
            _runner = (CommandRunnerHub) hub;
        }

        protected override void ProccessPacket(IPacket packet)
        {
            throw new NotImplementedException();
        }
    }
}
