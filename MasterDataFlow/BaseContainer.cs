using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;

namespace MasterDataFlow
{
    public abstract class BaseContainer : IContainer
    {
        // TODO is it necessary
        public abstract void Dispose();

        protected abstract void Execute(Guid loopId, ILoopCommandData data, EventLoopCallback callback);

        protected abstract void Subscribe(WorkflowKey workflowKey, SubscribeKey key);

        protected abstract void Unsubscribe(WorkflowKey workflowKey, SubscribeKey key);

        void IContainer.Subscribe(WorkflowKey workflowKey, SubscribeKey key)
        {
            Subscribe(workflowKey, key);
        }

        void IContainer.Unsubscribe(WorkflowKey workflowKey, SubscribeKey key)
        {
            Unsubscribe(workflowKey, key);
        }

        public void Execute(INotificationReceiverKey receiverKey, CommandInfo dataInfo)
        {
            
        }

    }
}
