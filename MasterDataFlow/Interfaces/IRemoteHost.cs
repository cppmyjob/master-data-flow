using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Interfaces
{
    public interface IRemoteHost
    {
        ICommandWorkflow RegisterWorkflow(WorkflowKey key, EventLoopCallback callback);

        void Run(ICommandWorkflow workflow, CommandKey commandKey, CommandDefinition commandDefinition, ICommandDataObject commandDataObject = null);
    }
}
