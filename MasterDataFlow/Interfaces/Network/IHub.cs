using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Interfaces.Network
{
    public enum SendStatus
    {
        Ok,
        Busy,
        Fault,
        NotSupported,
    }

    public interface IHub
    {
        BaseKey Key { get; }
        bool ConnectHub(IHub hub);
        // TODO rename AcceptHub
        void AcceptHub(IHub hub);
        void DisconnectHub(IHub hub);
        SendStatus Send(IPacket packet);
        IHubAccumulator Accumulator { get; }
        IList<IHub> ConnectedHubs { get; }
    }

}
