using System;

namespace MasterDataFlow.Network.Packets
{
    [Serializable]
    public class RemotePacket
    {
        private readonly string _senderKey;
        private readonly string _recieverKey;
        private readonly string _bodyType;
        private readonly string _body;

        public RemotePacket(string senderKey, string recieverKey, string bodyType, string body)
        {
            _senderKey = senderKey;
            _recieverKey = recieverKey;
            _bodyType = bodyType;
            _body = body;
        }

        public string TypeName
        {
            get { return _bodyType; }
        }

        public string SenderKey
        {
            get { return _senderKey; }
        }

        public string RecieverKey
        {
            get { return _recieverKey; }
        }

        public string Body
        {
            get { return _body; }
        }
    }
}
