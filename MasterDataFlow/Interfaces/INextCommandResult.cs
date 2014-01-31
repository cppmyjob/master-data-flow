using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Interfaces
{

    public interface INextCommandResult<out TCommandDataObject> : ICommandResult, IDataObjectHolder<TCommandDataObject>
        where TCommandDataObject : ICommandDataObject
    {
    }
}
