using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Actions;
using MasterDataFlow.Interfaces.Network;

namespace MasterDataFlow.Handlers
{
    public class ServerGateHandler : BaseHandler
    {
        public override string[] SupportedActions
        {
            get { return new [] {RemoteExecuteCommandAction.ActionName}; }
        }

        public override void Execute(string actionName, IPacket packet)
        {
            switch (actionName)
            {
                case RemoteExecuteCommandAction.ActionName:
                    RemoteExecuteCommandActionProcess((RemoteExecuteCommandAction)packet.Body);
                    break;
                default:
                    // TODO Send Error Exception
                    break;
            }
        }

        private void RemoteExecuteCommandActionProcess(RemoteExecuteCommandAction action)
        {
            throw new NotImplementedException();
        }
    }
}
