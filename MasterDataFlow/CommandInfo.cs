using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow
{
    public class CommandInfo
    {
        public CommandDefinition CommandDefinition { get; internal set; }
        public ICommandDataObject CommandDataObject { get; internal set; }
        internal OnExecuteCommand OnExecuteCommand { get; set; }
        internal OnChangeStatus OnChangeStatus { get; set; }
        public bool IsError { get; internal set; }
        public ICommandResult CommandResult { get; internal set; }
        public CommandDomain Domain { get; internal set; }
    }
}
