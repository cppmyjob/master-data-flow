using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Interfaces
{
    public interface IContainer : ILoopCommand, IDisposable
    {
        void Subscribe(Guid workflowId, TrackedKey key);

        void Unsubscribe(Guid workflowId, TrackedKey key);
    }
}
