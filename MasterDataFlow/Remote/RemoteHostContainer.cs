using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Remote
{
    public class RemoteHostContainer : IRemoteHostContract
    {
        private readonly ICommandDomainContainer _domainResolver;

        internal RemoteHostContainer(ICommandDomainContainer domainResolver)
        {
            _domainResolver = domainResolver;
        }

        public void UploadAssembly(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void Execute(Guid domainId, string typeName, string dataObject)
        {
            var domainInstance = _domainResolver.Find(domainId);
            //var commandType = Assembly.Load(typeName);
            domainInstance.Start(null, null);
        }
    }
}
