using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;

namespace MasterDataFlow.Interfaces
{
    public interface IRemoteClientContext
    {
        IRemoteHostContract Contract { get; }
        //void RegisterCallback(Guid loopId, EventLoopCallback callback);
    }
}
