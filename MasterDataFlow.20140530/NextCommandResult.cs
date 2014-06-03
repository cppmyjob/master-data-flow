using MasterDataFlow._20140530.Interfaces;

namespace MasterDataFlow._20140530
{
    public class NextCommandResult
    {
        private readonly ICommandDataObject _commandDataObject;
        private readonly CommandDefinition _definition;

        public NextCommandResult(CommandDefinition definition, ICommandDataObject commandDataObject)
        {
            _definition = definition;
            _commandDataObject = commandDataObject;
        }

        public CommandDefinition Definition
        {
            get { return _definition; }
        }

        public ICommandDataObject CommandDataObject
        {
            get { return _commandDataObject; }
        }
    }
}
