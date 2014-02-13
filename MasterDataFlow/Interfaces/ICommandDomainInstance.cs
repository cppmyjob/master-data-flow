using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Interfaces
{
    internal interface ICommandDomainInstance
    {
        ExecutionContext Start(Type commandType, ICommandDataObject commandDataObject);
    }
}
