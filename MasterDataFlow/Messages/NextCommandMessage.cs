using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Messages
{
    public class NextCommandMessage : ILoopCommandMessage
    {
        private readonly Guid _loopId;

        public NextCommandMessage(Guid loopId)
        {
            _loopId = loopId;
        }

        public Guid LoopId
        {
            get { return _loopId; }
        }
    }
}
