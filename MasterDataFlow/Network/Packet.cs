using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Network
{
    public class Packet : IPacket
    {
        private readonly BaseKey _senderKey;
        private readonly BaseKey _recieverKey;
        private readonly object _body;

        public Packet(BaseKey senderKey, BaseKey recieverKey, object body)
        {
            _senderKey = senderKey;
            _recieverKey = recieverKey;
            _body = body;
        }

        public BaseKey SenderKey
        {
            get { return _senderKey; }
        }

        public BaseKey RecieverKey
        {
            get { return _recieverKey; }
        }

        public object Body
        {
            get { return _body; }
        }
    }
}
