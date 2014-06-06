using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;

namespace MasterDataFlow.Interfaces
{
    public interface ILoopCommand
    {
        //ILoopCommandData Data { get; set; }
        void Execute(Guid loopId, ILoopCommandData data, EventLoopCallback callback);
    }
}
