using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Remote
{
    internal class RemoteHost : IRemoteHost, IDisposable
    {
        private readonly AsyncDictionary<WorkflowKey, CommandWorkflow> _workflows = new AsyncDictionary<WorkflowKey, CommandWorkflow>();
        private readonly CommandRunner _runner = new CommandRunner();

        public ICommandWorkflow RegisterWorkflow(WorkflowKey key, EventLoopCallback callback)
        {
            // TODO It's very not optimal locking need to rewrite
            lock (this)
            {
                var result = _workflows.GetItem(key);
                if (result == null)
                {
                    result = new CommandWorkflow(key, _runner);
                    result.MessageRecieved += (loopId, status, message) => callback(loopId, status, message);
                    _workflows.AddItem(key, result);
                }
                return result;
            }
        }

        public void AddContainter(BaseContainer container)
        {
            _runner.AddContainter(container);
        }


        public void Dispose()
        {
            // TODO implement right Dispose
            _runner.Dispose();
        }

        public void Run(Guid loopId, ICommandWorkflow workflow, CommandKey commandKey, CommandDefinition commandDefinition, ICommandDataObject commandDataObject = null)
        {
            _runner.Run(loopId, workflow, commandKey, commandDefinition, commandDataObject);
        }
    }
}
