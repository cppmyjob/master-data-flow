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
        public CommandDefinition CommandDefinition { get; internal set; }
        public ICommandDataObject CommandDataObject { get; internal set; }
        [Obsolete]
        public ICommandWorkflow CommandWorkflow { get; internal set; }

        public WorkflowKey WorkflowKey { get; internal set; }
        public CommandKey CommandKey { get; internal set; }
    }
}
