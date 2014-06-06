using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Results
{
    public class FaultCommandResult : ICommandResult
    {
        public Exception Exception { get; set; }

        public FaultCommandResult(Exception exception)
        {
            Exception = exception;
        }

        public NextCommandResult FindNextCommand(CommandDomain domain)
        {
            return null;
        }
    }
}
