using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Messages
{
    public class ResultCommandMessage : ILoopCommandMessage
    {
        private readonly ICommandResult _commandResult;

        public ResultCommandMessage(ICommandResult commandResult)
        {
            _commandResult = commandResult;
        }

        public ICommandResult CommandResult {
            get { return _commandResult; }
        }
    }
}
