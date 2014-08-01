using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;

namespace MasterDataFlow
{
    public abstract class BaseContainer : BaseEventLoop, IContainer
    {
        // TODO is it necessary
        public abstract void Dispose();

        protected abstract void Execute(Guid loopId, ILoopCommandData data, EventLoopCallback callback);

        void ILoopCommand.Execute(Guid loopId, ILoopCommandData data, EventLoopCallback callback)
        {
            Execute(loopId, data, callback);
        }

        protected abstract void Subscribe(Guid workflowId, TrackedKey key);

        protected abstract void Unsubscribe(Guid workflowId, TrackedKey key);

        void IContainer.Subscribe(Guid workflowId, TrackedKey key)
        {
            Subscribe(workflowId, key);
        }

        void IContainer.Unsubscribe(Guid workflowId, TrackedKey key)
        {
            Unsubscribe(workflowId, key);
        }

    }
}
