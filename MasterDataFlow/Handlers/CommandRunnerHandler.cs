using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Actions;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Handlers
{
    public class CommandRunnerHandler : BaseHandler
    {
        private AsyncDictionary<BaseKey, SimpleContainerHub> _containers = new AsyncDictionary<BaseKey, SimpleContainerHub>();

        public override string[] SupportedActions
        {
            get { return new[] { FindContainerAndLaunchCommandAction.ActionName}; }
        }

        public override void Execute(string actionName, IPacket packet)
        {
            switch (actionName)
            {
                case FindContainerAndLaunchCommandAction.ActionName:
                    ProcessFindContainerAndLaunchCommandAction((FindContainerAndLaunchCommandAction) packet.Body);
                    break;
            }
        }

        public override void ConnectHub(IHub hub)
        {
            if (hub is SimpleContainerHub)
            {
                _containers.AddItem(hub.Key, (SimpleContainerHub) hub);
            }
        }

        private void ProcessFindContainerAndLaunchCommandAction(FindContainerAndLaunchCommandAction action)
        {
            //
            throw new NotImplementedException();
        }
    }
}
