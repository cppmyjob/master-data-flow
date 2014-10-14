using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Network
{
    public class RemotePacket : Packet
    {
        private readonly string _bodyType;

        public RemotePacket(BaseKey senderKey, BaseKey recieverKey, string bodyType, object body)
            : base(senderKey, recieverKey, body)
        {
            _bodyType = bodyType;
        }

        public string TypeName
        {
            get { return _bodyType; }
        }
    }
}
