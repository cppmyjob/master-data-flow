using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;
using MasterDataFlow.Network.Packets;

namespace MasterDataFlow.Interfaces
{
    public interface IGateContract
    {
        void Send(RemotePacket packet);
    }
}
