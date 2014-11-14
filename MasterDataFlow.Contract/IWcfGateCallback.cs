using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using MasterDataFlow.Network;

namespace MasterDataFlow.Contract
{
    public interface IWcfGateCallback
    {
        [OperationContract(IsOneWay = true)]
        void Send(RemotePacket packet);
    }
}
