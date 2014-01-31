using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Results
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
