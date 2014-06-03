using System;
using MasterDataFlow._20140530.Interfaces;

namespace MasterDataFlow._20140530
{
    public class CommandInfo
    {
        public CommandDefinition CommandDefinition { get; internal set; }
        public ICommandDataObject CommandDataObject { get; internal set; }
        public Guid CommandDomainId { get; internal set; }
        //internal OnExecuteCommand OnExecuteCommand { get; set; }
        //internal OnChangeStatus OnChangeStatus { get; set; }
        public bool IsError { get; internal set; }
        public ICommandResult CommandResult { get; internal set; }
    }
}
