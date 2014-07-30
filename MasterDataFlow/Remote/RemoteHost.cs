using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Remote
{
    internal class RemoteHost : IRemoteHost, IDisposable
    {
        private readonly AsyncDictionary<Guid, CommandWorkflow> _workflows = new AsyncDictionary<Guid, CommandWorkflow>();
        private readonly CommandRunner _runner = new CommandRunner();

        public ICommandWorkflow RegisterWorkflow(Guid id, EventLoopCallback callback)
        {
            // TODO It's very not optimal locking need to rewrite
            lock (this)
            {
                var result = _workflows.GetItem(id);
                if (result == null)
                {
                    result = new CommandWorkflow(id, _runner);
                    result.MessageRecieved += (loopId, status, message) => callback(loopId, status, message);
                    _workflows.AddItem(id, result);
                }
                return result;
            }
        }

        public void AddContainter(BaseContainter container)
        {
            _runner.AddContainter(container);
        }


        public void Dispose()
        {
            // TODO implement right Dispose
            _runner.Dispose();
        }

        public void Run(Guid loopId, ICommandWorkflow workflow, CommandDefinition commandDefinition, ICommandDataObject commandDataObject = null)
        {
            _runner.Run(loopId, workflow, commandDefinition, commandDataObject);
        }
    }
}
