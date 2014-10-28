using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;

namespace MasterDataFlow.Interfaces
{
    public interface IGateContract
    {
        void UploadAssembly(byte[] data);
        void Send(RemotePacket packet);
    }
}
