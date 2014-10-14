using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Actions;
using MasterDataFlow.Handlers;

namespace MasterDataFlow.Network
{
    public class CommandRunnerHub : ActionHub
    {
        public CommandRunnerHub()
        {
            RegisterHandler(new CommandRunnerHandler());
        }
    }
}
