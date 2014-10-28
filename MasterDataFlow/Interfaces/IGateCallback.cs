using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Network;

namespace MasterDataFlow.Interfaces
{

    public interface IGateCallback
    {
        void Send(RemotePacket packet);
    }
}
