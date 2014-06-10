using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow
{
    public delegate void OnExecuteContainer(BaseContainter container, CommandInfo info);

    public abstract class BaseContainter : BaseEventLoop, ILoopCommand, IDisposable
    {
        public abstract void Dispose();

        public ILoopCommandData Data { get; set; }
        public abstract void Execute(Guid loopId, ILoopCommandData data, EventLoopCallback callback);
    }
}
