using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;

namespace MasterDataFlow
{
    public class CommandInfo : ILoopCommandData
    {
        public CommandDefinition CommandDefinition { get; internal set; }
        public ICommandDataObject CommandDataObject { get; internal set; }
        public ICommandWorkflow CommandWorkflow { get; internal set; }
        public CommandKey CommandKey { get; internal set; }
    }
}
