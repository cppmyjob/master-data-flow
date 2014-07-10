using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Results
{
    public class StopCommandResult : ICommandResult
    {
        public NextCommandResult FindNextCommand(ICommandDomain domain)
        {
            return null;
        }
    }
}
