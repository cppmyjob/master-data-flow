using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;

namespace MasterDataFlow.Contract
{
    [ServiceContract(CallbackContract = typeof(IWcfGateCallback))]
    public interface IWcfGateContract
    {
        [OperationContract]
        void Send(RemotePacket packet);
    }
}
