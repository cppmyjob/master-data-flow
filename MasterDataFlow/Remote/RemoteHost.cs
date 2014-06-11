using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Remote
{
    internal class RemoteHost : IDisposable
    {
// ReSharper disable InconsistentNaming
        private static readonly RemoteHost _instance = new RemoteHost();
// ReSharper restore InconsistentNaming

        public static RemoteHost Instance
        {
            get { return _instance; }
        }

        private readonly AsyncDictionary<Guid, CommandDomain> _domains = new AsyncDictionary<Guid, CommandDomain>();
        private readonly CommandRunner _runner = new CommandRunner();

        public CommandRunner Runner
        {
            get { return _runner; }
        }

        public CommandDomain RegisterDomain(Guid id)
        {
            // TODO It's very not optimal locking need to rewrite
            lock (_instance)
            {
                var result = _domains.GetItem(id) ?? new CommandDomain(id, Runner);
                return result;
            }
        }


        public void Dispose()
        {
            // TODO implement right Dispose
            Runner.Dispose();
        }
    }
}
