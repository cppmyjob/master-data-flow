using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Exceptions;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;
using MasterDataFlow.Utils;

namespace MasterDataFlow
{
    public class CommandRunner : BaseEventLoop, IDisposable
    {
        private readonly IList<BaseContainter> _containers = new List<BaseContainter>();
        private readonly AsyncQueue<BaseContainter> _freeContainers = new AsyncQueue<BaseContainter>();
        private bool _disposed = false; 
        private readonly Thread _commandThread;

        private class ProxyContainerCommand : ILoopCommand
        {
            private readonly CommandRunner _runner;
            private Guid _loopId;
            private EventLoopCallback _callback;
            private BaseContainter _containter;
            private readonly CommandInfo _data = new CommandInfo();

            public ProxyContainerCommand(CommandRunner runner)
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

            public ICommandWorkflow CommandWorkflow
            {
                get { return _data.CommandWorkflow; }
                internal set { _data.CommandWorkflow = value; }
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
                        _runner._freeContainers.Enqueue(_containter);
                        message = ProcessNextCommand(ref status, message);
                        if (message != null)
                        {
                            _callback(loopId, status, message);
                        }
                        break;
                    case EventLoopCommandStatus.Fault:
                        _runner._freeContainers.Enqueue(_containter);
                        _callback(loopId, status, message);
                        break;
                    case EventLoopCommandStatus.RemoteCall:
                        _runner._freeContainers.Enqueue(_containter);
                        _callback(loopId, status, null);
                        break;
                    case EventLoopCommandStatus.Progress:
                        throw new NotImplementedException();
                    default:
                        throw new Exception("ProxyContainerCommand exception 2");
                }
            }

            private ILoopCommandMessage ProcessNextCommand(ref EventLoopCommandStatus status, ILoopCommandMessage message)
            {
                if (message is DataCommandMessage)
                    return message;

                ICommandResult commandResult = ((ResultCommandMessage) message).CommandResult;
                if (commandResult == null)
                {
                    // TODO need check that all commands were completed
                    status = EventLoopCommandStatus.Fault;
                    message = new FaultCommandMessage(new WrongNextCommandException(""));
                    return message;
                }
                else
                {
                    var nextCommand = commandResult.FindNextCommand(_data.CommandWorkflow);
                    if (nextCommand == null)
                    {
                        ICommandDataObject commandDataObject = null;
                        var holder = commandResult as IDataObjectHolder<ICommandDataObject>;
                        if (holder != null)
                        {
                            commandDataObject = holder.DataObject;
                        }
                        message = new DataCommandMessage(commandDataObject);
                        return message;
                    }
                    else
                    {
                       var loopItem = _runner.CommandWaiting.GetItem(_loopId);
                       var newCommandLoopId = Guid.NewGuid();
                       _callback(_loopId, status, new NextCommandMessage(newCommandLoopId));
                       _runner.Run(newCommandLoopId, _data.CommandWorkflow, nextCommand.Definition, nextCommand.CommandDataObject, loopItem.InputCallback);
                        return null;
                    }
                }
            }
        }

        public CommandRunner()
        {
            _commandThread = new Thread(CommandProc);
            _commandThread.Start();
        }

        internal Guid Run(ICommandWorkflow workflow, CommandDefinition commandDefinition, ICommandDataObject commandDataObject = null, EventLoopCallback callback = null)
        {
            var loopId = Guid.NewGuid();
            Run(loopId, workflow, commandDefinition, commandDataObject, callback);
            return loopId;
        }

        internal void Run(Guid loopId, ICommandWorkflow workflow, CommandDefinition commandDefinition, ICommandDataObject commandDataObject, EventLoopCallback callback)
        {
            var command = new ProxyContainerCommand(this)
            {
                CommandDefinition = commandDefinition,
                CommandDataObject = commandDataObject,
                CommandWorkflow = workflow
            };
            Push(loopId, command, callback);
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
