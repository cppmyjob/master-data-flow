using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow
{
    internal class CommandDomainContainer : ICommandDomainContainer
    {
        public ICommandDomainInstance Find(Guid commadDomainId)
        {
            throw new NotImplementedException();
        }
    }
}
