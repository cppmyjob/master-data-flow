using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Results
{
    public class NextTypeCommandResult<TCommand> : INextCommandResult<ICommandDataObject>
        where TCommand : ICommand<ICommandDataObject>
    {
        private readonly ICommandDataObject _dataObject;

        public NextTypeCommandResult()
        {
            
        }

        public NextTypeCommandResult(ICommandDataObject dataObject)
        {
            _dataObject = dataObject;
        }

        public ICommandDataObject DataObject
        {
            get { return _dataObject; }
        }

        public NextCommandResult FindNextCommand(ICommandDomain domain)
        {
            var definition = domain.Find<TCommand>();
            var result = new NextCommandResult(definition, _dataObject);
            return result;
        }
    }
}
