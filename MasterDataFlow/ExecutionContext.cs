using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Exceptions;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Results;

namespace MasterDataFlow
{
    public class ExecutionContext : IDisposable
    {
        private readonly ManualResetEvent _waiter = new ManualResetEvent(false);
        private ExecuteStatus _status = ExecuteStatus.NotStarted;
        private bool _disposed;
        private ICommandDataObject _resultCommandDataObject;
        private readonly CommandRunner _runner;
        private readonly CommandDefinition _commandDefinition;
        private readonly ICommandDataObject _commandDataObject;
        private Exception _exception;
        private CommandDomain _domain;

        internal ExecutionContext(CommandRunner runner, CommandDomain domain, CommandDefinition commandDefinition,
            ICommandDataObject commandDataObject)
        {
            _runner = runner;
            _domain = domain;
            _commandDefinition = commandDefinition;
            _commandDataObject = commandDataObject;
        }

        internal void Execute()
        {
            _status = ExecuteStatus.Progress;
            _runner.Run(_domain, _commandDefinition, _commandDataObject, OnChangeStatus);
        }

        public void OnChangeStatus(ExecuteStatus status, Exception exception, ICommandDataObject dataObject)
        {
            _status = status;
            _resultCommandDataObject = null;
            _exception = null;
            switch (status)
            {
                case ExecuteStatus.Fault:
                    _exception = exception;
                    _waiter.Set();
                    break;
                case ExecuteStatus.Completed:
                    _resultCommandDataObject = dataObject;
                    _waiter.Set();
                    break;
            }
        }

        public WaitHandle GetWaiter()
        {
            return _waiter;
        }

        public ICommandDataObject Result
        {
            get
            {
                return _resultCommandDataObject;
            }
        }

        public ExecuteStatus Status
        {
            get { return _status; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        public CommandDomain Domain
        {
            get { return _domain; }
        }

        // Implement IDisposable. 
        // Do not make this method virtual. 
        // A derived class should not be able to override this method. 
        void IDisposable.Dispose()
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
                    _runner.Dispose();
                    _waiter.Close();
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
