using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow
{
    public abstract class BaseCommand 
    {
        internal protected abstract ICommandResult BaseExecute();
    }
}
