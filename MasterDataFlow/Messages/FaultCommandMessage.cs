using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Messages
{
    public class FaultCommandMessage : ILoopCommandMessage
    {
        private readonly Exception _exception;

        public FaultCommandMessage(Exception exception)
        {
            _exception = exception;
        }

        public Exception Exception
        {
            get { return _exception; }
        }
    }
}
