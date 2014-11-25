using MasterDataFlow.Keys;

namespace MasterDataFlow.Network.Routing
{
    public enum PacketType
    {
        RouteRequest,
        RouteResponse
    }

    public class RequestPacket : Packet
    {
        private readonly string _requestId;
        private readonly PacketType _packetType;

        public RequestPacket(BaseKey senderKey, BaseKey recieverKey, object body, string requestId, PacketType type)
            : base(senderKey, recieverKey, body)
        {
            _requestId = requestId;
            _packetType = type;
        }

        public string RequestId
        {
            get { return _requestId; }
        }

        public PacketType PacketType
        {
            get { return _packetType; }
        }
    }
}
