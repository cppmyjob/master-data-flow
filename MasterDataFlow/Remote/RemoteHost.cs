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

        private readonly AsyncDictionary<Guid, CommandDomain> _domains = new AsyncDictionary<Guid, CommandDomain>();
        private readonly CommandRunner _runner = new CommandRunner();

        public ICommandDomain RegisterDomain(Guid id)
        {
            // TODO It's very not optimal locking need to rewrite
            lock (this)
            {
                var result = _domains.GetItem(id) ?? new CommandDomain(id, _runner);
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

        public void Run(Guid loopId, ICommandDomain domain, CommandDefinition commandDefinition, ICommandDataObject commandDataObject = null, EventLoop.EventLoopCallback callback = null)
        {
            _runner.Run(loopId, domain, commandDefinition, commandDataObject, callback);
        }
    }
}
