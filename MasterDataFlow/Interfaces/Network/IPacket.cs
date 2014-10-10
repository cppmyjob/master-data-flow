using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Interfaces.Network
{
    public interface IPacket
    {
        BaseKey SenderKey { get; }
        BaseKey RecieverKey { get; }
        object Body { get; }
    }
}
