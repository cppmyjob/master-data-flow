using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Network;

namespace MasterDataFlow.Interfaces
{

    public interface IRemoteCallback
    {
        //void Callback(string loopId, EventLoopCommandStatus status, string messageTypeName, string messageData);
        void Send(RemotePacket packet);
    }
}
