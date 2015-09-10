using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;

namespace MasterDataFlow
{
    public class LocalDomainCommandInfo
    {
        public Type CommandType { get; internal set; }
        public ICommandDataObject CommandDataObject { get; internal set; }
        public WorkflowKey WorkflowKey { get; internal set; }
        public CommandKey CommandKey { get; internal set; }
        //public IInstanceFactory InstanceFactory { get; internal set; }
    }
}
