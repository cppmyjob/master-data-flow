using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Messages
{
    public class DataCommandMessage : ILoopCommandMessage
    {
        private readonly ICommandDataObject _data;

        public DataCommandMessage(ICommandDataObject data)
        {
            _data = data;
        }

        public ICommandDataObject Data
        {
            get { return _data; }
        }
    }
}
