using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MasterDataFlow.Exceptions;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Remote.Commands;
using MasterDataFlow.Serialization;

namespace MasterDataFlow.Remote
{
    public class RemoteHostContainer : IRemoteHostContract
    {
        private readonly IRemoteContext _context;

        internal RemoteHostContainer(IRemoteContext context)
        {
            _context = context;
        }

        public void UploadAssembly(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void Execute(Guid requestId, Guid domainId, string commandTypeName, string dataObjectTypeName, string dataObject)
        {
            var command = new ExecuteCommand(_context, requestId, domainId, commandTypeName, dataObjectTypeName, dataObject);
            //_context.Queue.Push(command);
        }
    }
}
