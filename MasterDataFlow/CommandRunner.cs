using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Exceptions;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow
{
    public delegate void OnExecuteCommand(CommandInfo previousCommand);
    public delegate void OnChangeStatus(ExecuteStatus status, Exception exception, ICommandDataObject dataObject);

    internal class CommandRunner : IDisposable
    {
        private readonly IList<BaseContainter> _containers = new List<BaseContainter>();
        private readonly AsyncQueue<BaseContainter> _freeContainers = new AsyncQueue<BaseContainter>();
        private bool _disposed = false;
        private readonly Thread _commandThread;
        private readonly AsyncQueue<CommandInfo> _queue = new AsyncQueue<CommandInfo>();
        private readonly CommandDomain _domain;

        internal CommandRunner(CommandDomain domain)
        {
            _domain = domain;
            _commandThread = new Thread(CommandProc);
            _commandThread.Start();
        }

        public void AddContainter(BaseContainter container)
        {
            _containers.Add(container);
            _freeContainers.Enqueue(container);
        }

        public void Run(CommandDefinition commandDefinition, ICommandDataObject commandDataObject, OnChangeStatus onChangeStatus)
        {
            var info = new CommandInfo
            {
                CommandDefinition = commandDefinition,
                CommandDataObject = commandDataObject,
                CommandDomainId = _domain.Id,
                OnExecuteCommand = ProcessNextCommand,
                OnChangeStatus = onChangeStatus,
            };
            _queue.Enqueue(info);
        }

        private void ProcessNextCommand(CommandInfo previousCommandInfo)
        {
            if (previousCommandInfo.CommandResult == null)
            {
                // TODO need check that all commands were completed
                previousCommandInfo.OnChangeStatus(ExecuteStatus.Fault, new WrongNextCommandException(""), null);
            }
            else
            {
                var nextCommand = previousCommandInfo.CommandResult.FindNextCommand(_domain);
                if (nextCommand == null)
                {
                    ICommandDataObject commandDataObject = null;
                    var holder = previousCommandInfo.CommandResult as IDataObjectHolder<ICommandDataObject>;
                    if (holder != null)
                    {
                        commandDataObject = holder.DataObject;
                    }
                    previousCommandInfo.OnChangeStatus(ExecuteStatus.Completed, null, commandDataObject);
                }
                else
                {
                    Run(nextCommand.Definition, nextCommand.CommandDataObject, previousCommandInfo.OnChangeStatus);
                }                
            }
        }

        private void CommandProc()
        {
            while (true)
            {
                // TODO it seems that loop need not. it may work via ManualReset Event
                if (_queue.Count > 0 && _freeContainers.Count > 0)
                {
                    var container = _freeContainers.Dequeue();
                    var info = _queue.Dequeue();
                    container.Execute(info, OnExecuteContainer);

                }
                Thread.Sleep(50);
            }
        }

        private void OnExecuteContainer(BaseContainter container, CommandInfo info)
        {
            _freeContainers.Enqueue(container);
            info.OnExecuteCommand(info);
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
