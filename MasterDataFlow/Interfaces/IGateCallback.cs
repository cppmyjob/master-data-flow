using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Network;
using MasterDataFlow.Network.Packets;

namespace MasterDataFlow.Interfaces
{

    public interface IGateCallback
    {
        void Send(RemotePacket packet);
    }
}
