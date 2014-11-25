using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Actions;
using MasterDataFlow.Handlers;
using MasterDataFlow.Interfaces.Network;

namespace MasterDataFlow.Network
{
    public class CommandRunnerHub : ActionHub
    {
        public CommandRunnerHub()
        {
            RegisterHandler(new CommandRunnerHandler());
        }

        public override void AcceptHub(IHub hub)
        {
            base.AcceptHub(hub);
            // TODO call when hub is ServerGate
            //throw new NotSupportedException();
        }

    }
}
