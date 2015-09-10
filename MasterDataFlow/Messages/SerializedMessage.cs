using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Messages
{
    [Serializable]
    public class SerializedCommandMessage : CommandMessage
    {
        public SerializedCommandMessage(CommandKey key) : base(key)
        {
        }

        public string Data { get; set; }
        public Type DataType { get; set; }
    }
}
