using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;

namespace MasterDataFlow.Interfaces
{

    public delegate void GateCallbackPacketRecievedHandler(RemotePacket packet);

    public interface IClientContext : IDisposable
    {
        IGateContract Contract { get; }
        BaseKey ServerGateKey { get; }
        bool IsNeedSendKey { get; }
        event GateCallbackPacketRecievedHandler GateCallbackPacketRecieved;
    }
}
