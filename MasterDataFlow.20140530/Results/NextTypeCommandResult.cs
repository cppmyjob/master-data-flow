using MasterDataFlow._20140530.Interfaces;

namespace MasterDataFlow._20140530.Results
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

        public NextCommandResult FindNextCommand(CommandDomain domain)
        {
            var definition = domain.Find<TCommand>();
            var result = new NextCommandResult(definition, _dataObject);
            return result;
        }
    }
}
