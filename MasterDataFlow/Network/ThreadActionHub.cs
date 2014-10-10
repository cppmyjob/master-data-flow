using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Actions;
using MasterDataFlow.Handlers;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Network
{
    public class ThreadActionHub : Hub, IDisposable
    {
        private readonly ServiceKey _key = new ServiceKey();
        private readonly Thread _commandThread;
        private bool _disposed = false; 
        private readonly AsyncDictionary<string, BaseHandler> _handlers = new AsyncDictionary<string, BaseHandler>();
        private readonly List<BaseHandler> _unuqueHandlers = new List<BaseHandler>();

        public ThreadActionHub()
        {
            _commandThread = new Thread(CommandProc);
            _commandThread.Start();

        }

        public override BaseKey Key
        {
            get { return _key; }
        }

        public override bool ConnectHub(IHub hub)
        {
            var result = base.ConnectHub(hub);
            foreach (var handler in _unuqueHandlers)
            {
                handler.ConnectHub(hub);
            }
            return result;
        }

        protected void RegisterHandler(BaseHandler handler)
        {
            foreach (var supportedAction in handler.SupportedActions)
            {
                _handlers.AddItem(supportedAction, handler);
            }
            _unuqueHandlers.Add(handler);
        }

        protected override void ProccessPacket(IPacket packet)
        {
            if (packet.Body == null)
                // TODO Exception?
                return;
            var action = packet.Body as BaseAction;
            if (action == null)
                // TODO Exception?
                return;
            var handler = _handlers.GetItem(action.Name);
            if (handler == null)
                // TODO Exception?
                return;
            handler.Execute(action.Name, packet);
        }

        private void CommandProc()
        {
            while (true)
            {
                try
                {
                    // TODO need to improve catch, Sleep move to another place
                    if (!Loop())
                        Thread.Sleep(50);
                }
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort();
                    break;
                }
            }
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
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    _commandThread.Abort();
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.

                // Note disposing has been done.
                _disposed = true;

            }
        }
    }
}
