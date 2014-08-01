using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;

namespace MasterDataFlow
{
    public abstract class BaseCommand 
    {
        internal protected abstract ICommandResult BaseExecute();

        protected virtual void SendMessage(TrackedKey key)
        {
            
        }

        protected virtual void OnSubscribed(TrackedKey key)
        {

        }

        protected virtual void OnUnsubscribed(TrackedKey key)
        {
            
        }
    }
}
