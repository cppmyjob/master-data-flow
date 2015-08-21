using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Messages
{
    [Serializable]
    public class CommandMessage : BaseMessage
    {
        private readonly CommandKey _key;

        public CommandMessage(CommandKey key)
        {
            _key = key;
        }

        public CommandKey Key
        {
            get { return _key; }
        }
    }
}
