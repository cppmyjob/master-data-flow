using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;

namespace MasterDataFlow.Interfaces
{

    public interface IRemoteCallback
    {
        void Callback(string loopId, EventLoopCommandStatus status, string messageTypeName, string messageData);
    }
}
