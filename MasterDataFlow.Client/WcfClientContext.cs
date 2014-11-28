using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Contract;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Network.Packets;

namespace MasterDataFlow.Client
{
    public class WcfClientContext : IClientContext, IGateCallback
    {
        private readonly ServiceKey _serverGateKey;
        // Flag: Has Dispose already been called? 
        private bool _disposed = false;
        private readonly WcfClient _client;

        public WcfClientContext(ServiceKey serverGateKey)
        {
            _serverGateKey = serverGateKey;
            _client = new WcfClient(this);
        }

        public IGateContract Contract
        {
            get { return _client.Channel; }
        }

        public BaseKey ServerGateKey
        {
            get { return _serverGateKey; }
        }

        public bool IsNeedSendKey { get { return true; } }

        public event GateCallbackPacketRecievedHandler GateCallbackPacketRecieved;

       // Public implementation of Dispose pattern callable by consumers. 
       public void Dispose()
       { 
          Dispose(true);
          GC.SuppressFinalize(this);           
       }

       // Protected implementation of Dispose pattern. 
       protected virtual void Dispose(bool disposing)
       {
          if (_disposed)
             return; 

          if (disposing) {
              // Free any other managed objects here. 
              //
              _client.Dispose();
          }

          // Free any unmanaged objects here. 
          //
          _disposed = true;
       }

       ~WcfClientContext()
       {
          Dispose(false);
       }

       void IGateCallback.Send(RemotePacket packet)
       {
           if (GateCallbackPacketRecieved != null)
           {
               GateCallbackPacketRecieved(packet);
           }
       }

    }
}
