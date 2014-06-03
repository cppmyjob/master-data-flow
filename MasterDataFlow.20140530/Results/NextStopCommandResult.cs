using MasterDataFlow._20140530.Interfaces;

namespace MasterDataFlow._20140530.Results
{
    public class NextStopCommandResult<TCommandDataObject> : StopCommandResult, INextCommandResult<TCommandDataObject>
        where TCommandDataObject : ICommandDataObject
    {
        private readonly TCommandDataObject _dataObject;

        public NextStopCommandResult(TCommandDataObject dataObject)
        {
            _dataObject = dataObject;
        }

        public TCommandDataObject DataObject
        {
            get { return _dataObject; }
        }


    }
}
