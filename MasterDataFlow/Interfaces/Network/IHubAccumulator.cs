using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Interfaces.Network
{
    public enum HubAccumulatorStatus
    {
        Free,
        Busy,
        Extracted
    }

    public interface IHubAccumulator
    {
        void Lock(string key);
        void UnLock(string key);
        HubAccumulatorStatus GetStatus(string key);
        void Add(string key, IPacket packet);
        void SetBusyStatus(string key);
        List<IPacket> Extract(string key);
    }
}
