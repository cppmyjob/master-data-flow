using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;

namespace MasterDataFlow.Interfaces
{
    public interface IRemoteHost
    {
        ICommandDomain RegisterDomain(Guid id);

        void Run(Guid loopId, ICommandDomain domain, CommandDefinition commandDefinition, ICommandDataObject commandDataObject = null,
            EventLoopCallback callback = null);
    }
}
