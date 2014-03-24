using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Remote.Commands
{
    public abstract class RequestCommand<TCommandDataObject> : Command<TCommandDataObject>
        where TCommandDataObject : ExecuteCommand.RequestParams
    {
        private readonly IRemoteContext _remoteContext;

        internal RequestCommand(IRemoteContext remoteContext)
        {
            _remoteContext = remoteContext;
        }

        public class RequestParams : ICommandDataObject
        {
            public Guid RequestId { get; set; }
        }

        internal IRemoteContext RemoteContext
        {
            get { return _remoteContext; }
        }
    }
}
