using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Messages;

namespace MasterDataFlow.Interfaces
{
    public interface ICommand<out TCommandDataObject> : IDataObjectHolder<TCommandDataObject> 
        where TCommandDataObject : ICommandDataObject
    {
        BaseMessage Execute();
    }
}
