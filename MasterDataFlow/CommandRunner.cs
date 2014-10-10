using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Exceptions;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Utils;

namespace MasterDataFlow
{
    public class CommandRunner : BaseEventLoop, IDisposable, INotificationReciever
    {
        private readonly IList<IContainer> _containers = new List<IContainer>();
        private readonly AsyncQueue<IContainer> _freeContainers = new AsyncQueue<IContainer>();
        private bool _disposed = false; 
        private readonly Thread _commandThread;
        private readonly AsyncDictionary<INotificationReceiverKey, INotificationReciever> _notificationRecievers 
            = new AsyncDictionary<INotificationReceiverKey, INotificationReciever>();

        private readonly RunnerKey _key = new RunnerKey();

        //private class ProxyContainerCommand : ILoopCommand
        //{
        //    private readonly CommandRunner _runner;
        //    private Guid _loopId;
        //    private EventLoopCallback _callback;
        //    private IContainer _containter;
        //    private readonly CommandInfo _data = new CommandInfo();

        //    public ProxyContainerCommand(CommandRunner runner)
        //    {
        //        _runner = runner;
        //    }

        //    public CommandKey CommandKey
        //    {
        //        get { return _data.CommandKey; }
        //        internal set { _data.CommandKey = value; }
        //    }


        //    public CommandDefinition CommandDefinition
        //    {
        //        get { return _data.CommandDefinition; }
        //        internal set { _data.CommandDefinition = value; }
        //    }

        //    public ICommandDataObject CommandDataObject
        //    {
        //        get { return _data.CommandDataObject; }
        //        internal set { _data.CommandDataObject = value; }
        //    }

        //    public ICommandWorkflow CommandWorkflow
        //    {
        //        get { return _data.CommandWorkflow; }
        //        internal set { _data.CommandWorkflow = value; }
        //    }


        //    public void Execute(Guid loopId, ILoopCommandData data, EventLoopCallback callback)
        //    {
        //        _loopId = loopId;
        //        _callback = callback;
        //        // TODO it seems that loop need not. it may work via ManualReset Event
        //        if (_runner._freeContainers.Count > 0)
        //        {
        //            _containter = _runner._freeContainers.Dequeue();
        //            _containter.Execute(loopId, _data, WaitingCallback);
        //        }
        //    }

        //    private void WaitingCallback(Guid loopId, EventLoopCommandStatus status, ILoopCommandMessage message)
        //    {
        //        if (_loopId != loopId)
        //        {
        //            throw new Exception("ProxyContainerCommand exception 1");
        //        }
        //        switch (status)
        //        {
        //            case EventLoopCommandStatus.Completed:
        //                _runner._freeContainers.Enqueue(_containter);
        //                message = ProcessNextCommand(ref status, message);
        //                if (message != null)
        //                {
        //                    _callback(loopId, status, message);
        //                }
        //                break;
        //            case EventLoopCommandStatus.Fault:
        //                _runner._freeContainers.Enqueue(_containter);
        //                _callback(loopId, status, message);
        //                break;
        //            case EventLoopCommandStatus.RemoteCall:
        //                _runner._freeContainers.Enqueue(_containter);
        //                _callback(loopId, status, null);
        //                break;
        //            case EventLoopCommandStatus.Progress:
        //                throw new NotImplementedException();
        //            default:
        //                throw new Exception("ProxyContainerCommand exception 2");
        //        }
        //    }

        //    private ILoopCommandMessage ProcessNextCommand(ref EventLoopCommandStatus status, ILoopCommandMessage message)
        //    {
        //        if (message is DataCommandMessage)
        //            return message;

        //        ICommandResult commandResult = ((ResultCommandMessage) message).CommandResult;
        //        if (commandResult == null)
        //        {
        //            // TODO need check that all commands were completed
        //            status = EventLoopCommandStatus.Fault;
        //            message = new FaultCommandMessage(new WrongNextCommandException(""));
        //            return message;
        //        }
        //        else
        //        {
        //            var nextCommand = commandResult.FindNextCommand(_data.CommandWorkflow);
        //            if (nextCommand == null)
        //            {
        //                ICommandDataObject commandDataObject = null;
        //                var holder = commandResult as IDataObjectHolder<ICommandDataObject>;
        //                if (holder != null)
        //                {
        //                    commandDataObject = holder.DataObject;
        //                }
        //                message = new DataCommandMessage(commandDataObject);
        //                return message;
        //            }
        //            else
        //            {
        //               var loopItem = _runner.CommandWaiting.GetItem(_loopId);
        //               var newCommandLoopId = Guid.NewGuid();
        //               _callback(_loopId, status, new NextCommandMessage(newCommandLoopId));
        //               _runner.Run(newCommandLoopId, _data.CommandWorkflow, _data.CommandKey, nextCommand.Definition, nextCommand.CommandDataObject);
        //                return null;
        //            }
        //        }
        //    }
        //}

        private class ProxyContainerCommand : ILoopCommand
        {
            private readonly CommandRunner _runner;
            private readonly CommandInfo _data = new CommandInfo();

            public ProxyContainerCommand(CommandRunner runner)
            {
                _runner = runner;
            }

            public CommandKey CommandKey
            {
                get { return _data.CommandKey; }
                internal set { _data.CommandKey = value; }
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

            public bool Execute(INotificationReceiverKey notificationKey, EventLoopCallback callback)
            {
                if (_runner._freeContainers.Count == 0) 
                    return false;
                var containter = _runner._freeContainers.Dequeue();
                containter.Execute(notificationKey, _data);
                return true;
            }
        }

        public CommandRunner()
        {
            _commandThread = new Thread(CommandProc);
            _commandThread.Start();
            _notificationRecievers.AddItem(_key, this);
        }

        INotificationReceiverKey INotificationReciever.RecieverKey
        {
            get { return _key; }
        }

        void INotificationReciever.Notify(object data)
        {
            throw new NotImplementedException();
        }

        internal void RegisterNotificationReceiver(INotificationReciever reciever)
        {
            _notificationRecievers.AddItem(reciever.RecieverKey, reciever);
        }

        internal void Run(ICommandWorkflow workflow, CommandKey commandKey, CommandDefinition commandDefinition, ICommandDataObject commandDataObject = null)
        {
            var command = new ProxyContainerCommand(this)
            {
                CommandWorkflow = workflow,
                CommandKey = commandKey,
                CommandDefinition = commandDefinition,
                CommandDataObject = commandDataObject
            };
            Push(((INotificationReciever)this).RecieverKey, command);
        }

        internal void Subscribe(ICommandWorkflow workflow)
        {
            
        }

        public void AddContainter(IContainer container)
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
