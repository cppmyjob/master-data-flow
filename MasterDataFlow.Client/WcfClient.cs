﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using MasterDataFlow.Contract;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Network;
using MasterDataFlow.Network.Packets;

namespace MasterDataFlow.Client
{
    public class WcfClient : IWcfGateCallback, IDisposable
    {
        private readonly ClientInternal _client;
        private bool _disposed = false;
        //private readonly IWcfGateContract _channel;
        private IGateCallback _callback;

        private class ClientInternal : System.ServiceModel.ClientBase<IWcfGateContract>, IGateContract
        {
            public ClientInternal(InstanceContext context) : base(context)
            {
            }

            public void Send(RemotePacket packet)
            {
                Channel.Send(packet);
            }
        }

        public WcfClient(IGateCallback callback)
        {
            _callback = callback;
            //_client = new ClientInternal();

            var ctx = new InstanceContext(this);
            _client = new ClientInternal(ctx);
            //_channel = new DuplexChannelFactory<IWcfGateContract>(ctx).CreateChannel();

        }

        public IGateContract Channel
        {
            get { return _client; }
        }


        // Implement IDisposable. 
        // Do not make this method virtual. 
        // A derived class should not be able to override this method. 
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    ((IDisposable)_client).Dispose();

                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.

                // Note disposing has been done.
                _disposed = true;

            }
        }

        // Use C# destructor syntax for finalization code. 
        // This destructor will run only if the Dispose method 
        // does not get called. 
        // It gives your base class the opportunity to finalize. 
        // Do not provide destructors in types derived from this class.
        ~WcfClient()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }

        void IWcfGateCallback.Send(RemotePacket packet)
        {
            _callback.Send(packet);
        }

    }
}
