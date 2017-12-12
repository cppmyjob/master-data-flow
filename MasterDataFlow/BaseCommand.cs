using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;

namespace MasterDataFlow
{

    public abstract class BaseCommand 
    {
        // TODO move setting key value to constructor
        public CommandKey Key { get; internal set; }
        public WorkflowKey CreatorWorkflowKey { get; internal set; }

        public IMessageSender MessageSender { get; internal set; }

        protected internal abstract BaseMessage BaseExecute();

        protected virtual void SendMessage(BaseKey recipient, CommandMessage message)
        {
            MessageSender.Send(recipient, message);
        }

        protected virtual void OnSubscribed(BaseKey key)
        {

        }

        protected virtual void OnUnsubscribed(BaseKey key)
        {

        }
    }
}
