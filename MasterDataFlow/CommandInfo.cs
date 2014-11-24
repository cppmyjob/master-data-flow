using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;

namespace MasterDataFlow
{
    public class CommandInfo
    {
        public Type CommandType { get; internal set; }
        public ICommandDataObject CommandDataObject { get; internal set; }
        public WorkflowKey WorkflowKey { get; internal set; }
        public CommandKey CommandKey { get; internal set; }
        public ICommandFactory CommandFactory { get; internal set; }
    }
}
