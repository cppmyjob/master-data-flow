using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Remote
{
    internal class RemoteHost : IRemoteHost, IDisposable
    {
//// ReSharper disable InconsistentNaming
//        private static readonly RemoteHost _instance = new RemoteHost();
//// ReSharper restore InconsistentNaming

//        public static RemoteHost Instance
//        {
//            get { return _instance; }
//        }

        private readonly AsyncDictionary<Guid, CommandWorkflow> _workflows = new AsyncDictionary<Guid, CommandWorkflow>();
        private readonly CommandRunner _runner = new CommandRunner();

        public ICommandWorkflow RegisterWorkflow(Guid id)
        {
            // TODO It's very not optimal locking need to rewrite
            lock (this)
            {
                var result = _workflows.GetItem(id) ?? new CommandWorkflow(id, _runner);
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

        public void Run(Guid loopId, ICommandWorkflow workflow, CommandDefinition commandDefinition, ICommandDataObject commandDataObject = null, EventLoop.EventLoopCallback callback = null)
        {
            _runner.Run(loopId, workflow, commandDefinition, commandDataObject, callback);
        }
    }
}
