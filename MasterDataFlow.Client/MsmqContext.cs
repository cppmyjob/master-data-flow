using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Network.Packets;

namespace MasterDataFlow.Client
{
    public class MsmqContext : IClientContext, IGateCallback
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IGateContract Contract { get; private set; }
        public BaseKey ServerGateKey { get; private set; }
        public bool IsNeedSendKey { get; private set; }
        public event GateCallbackPacketRecievedHandler GateCallbackPacketRecieved;

        public void Send(RemotePacket packet)
        {
            throw new NotImplementedException();
        }
    }
}
