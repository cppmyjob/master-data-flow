﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Utils;

namespace MasterDataFlow
{
    internal class CommandEventLoopRunner : BaseEventLoop, IDisposable
    {
        private readonly IList<BaseContainter> _containers = new List<BaseContainter>();
        private readonly AsyncQueue<BaseContainter> _freeContainers = new AsyncQueue<BaseContainter>();
        private readonly CommandDomain _domain;
        private bool _disposed = false; 
        private readonly Thread _commandThread;

        private class ProxyContainerCommand : ILoopCommand
        {
            private readonly CommandEventLoopRunner _runner;
            private Guid _loopId;
            private EventLoopCallback _callback;
            private BaseContainter _containter;
            private readonly CommandInfo _data = new CommandInfo();

            public ProxyContainerCommand(CommandEventLoopRunner runner)
            {
                _runner = runner;
            }

            public CommandDefinition CommandDefinition
            {
                get { return _data.CommandDefinition; }
                internal set { _data.CommandDefinition = value; }
            }

            public ICommandDataObject CommandDataObject
            {
                get { return _data.CommandDataObject; }
                internal set { _data.CommandDataObject = value; }
            }

            public void Execute(Guid loopId, ILoopCommandData data, EventLoopCallback callback)
            {
                _loopId = loopId;
                _callback = callback;
                // TODO it seems that loop need not. it may work via ManualReset Event
                if (_runner._freeContainers.Count > 0)
                {
                    _containter = _runner._freeContainers.Dequeue();
                    _containter.Execute(loopId, _data, WaitingCallback);
                }
            }

            private void WaitingCallback(Guid loopId, EventLoopCommandStatus status, ILoopCommandMessage message)
            {
                if (_loopId != loopId)
                {
                    throw new Exception("ProxyContainerCommand exception 1");
                }
                switch (status)
                {
                    case EventLoopCommandStatus.Completed:
                    case EventLoopCommandStatus.Fault:
                        _runner._freeContainers.Enqueue(_containter);
                        break;
                    case EventLoopCommandStatus.Progress:
                        break;
                    default:
                        throw new Exception("ProxyContainerCommand exception 2");
                }
                _callback(loopId, status, message);
            }
        }

        internal CommandEventLoopRunner(CommandDomain domain)
        {
            _domain = domain;
            _commandThread = new Thread(CommandProc);
            _commandThread.Start();
        }

        public Guid Run(CommandDefinition commandDefinition, ICommandDataObject commandDataObject = null, EventLoopCallback callback = null)
        {
            var command = new ProxyContainerCommand(this)
            {
                CommandDefinition = commandDefinition,
                CommandDataObject = commandDataObject
            };
            var loopId = Push(command, callback);
            return loopId;
        }


        public void AddContainter(BaseContainter container)
        {
            _containers.Add(container);
            _freeContainers.Enqueue(container);
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