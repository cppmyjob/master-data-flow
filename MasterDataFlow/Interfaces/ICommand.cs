using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Interfaces
{
    public interface ICommand<out TCommandDataObject> : IDataObjectHolder<TCommandDataObject> 
        where TCommandDataObject : ICommandDataObject
    {
        INextCommandResult<ICommandDataObject> Execute();
    }
}
