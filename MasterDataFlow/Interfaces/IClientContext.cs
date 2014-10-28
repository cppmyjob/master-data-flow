using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Interfaces
{
    public interface IClientContext : IDisposable
    {
        IGateContract Contract { get; }
        BaseKey ServerGateKey { get; }
    }
}
