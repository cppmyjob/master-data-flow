using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Exceptions;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow
{
    public class CommandDomainInstance : ICommandDomainInstance
    {
        private readonly CommandRunner _commandRunner;
        private bool _disposed;
        private bool _isRunning;
        private ExecutionContext _currentContext;
        private readonly CommandDomain _commandDomain;

        public CommandDomainInstance(CommandDomain commandDomain)
        {
            _commandDomain = commandDomain;
            _commandRunner = new CommandRunner(_commandDomain);
        }

        public CommandDomain CommandDomain
        {
            get { return _commandDomain; }
        }

        public void AddContainter(BaseContainter container)
        {
            _commandRunner.AddContainter(container);
        }

        public ExecutionContext Start<TCommand>(ICommandDataObject commandDataObject)
            where TCommand : ICommand<ICommandDataObject>
        {
            var commandType = typeof(TCommand);
            return ((ICommandDomainInstance)this).Start(commandType, commandDataObject);
        }

        ExecutionContext ICommandDomainInstance.Start(Type commandType, ICommandDataObject commandDataObject)
        {
            if (_isRunning)
                throw new IsAlreadyRunningException();

            var commandDefinition = _commandDomain.Find(commandType);
            // TODO check if commandDefinition was found

            _currentContext = new ExecutionContext(_commandRunner, commandDefinition, commandDataObject);
            _currentContext.Execute();
            // TODO is it thread safe
            _isRunning = true;
            return _currentContext;
        }

        public ExecutionContext Stop()
        {
            throw new NotImplementedException();
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
                    _commandRunner.Dispose();

                    if (_currentContext != null)
                        // TODO is it thread safe
                        ((IDisposable)_currentContext).Dispose();

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
