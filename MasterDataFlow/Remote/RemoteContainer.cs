using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Remote
{
    public class RemoteContainer : BaseContainter
    {
        private readonly IRemoteHostContract _remoteHostContract;

        public RemoteContainer(IRemoteHostContract remoteHostContract)
        {
            _remoteHostContract = remoteHostContract;
        }

        public override void Execute(CommandInfo info, OnExecuteContainer onExecute)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            
        }
    }
}
