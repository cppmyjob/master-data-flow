using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;

namespace MasterDataFlow.Interfaces
{
    public interface IRemoteHost
    {
        ICommandWorkflow RegisterWorkflow(Guid id, EventLoopCallback callback);

        void Run(Guid loopId, ICommandWorkflow workflow, CommandDefinition commandDefinition, ICommandDataObject commandDataObject = null);
    }
}
