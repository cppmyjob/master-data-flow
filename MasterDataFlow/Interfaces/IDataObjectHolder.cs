using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Interfaces
{
    public interface IDataObjectHolder<out TCommandDataObject>
        where TCommandDataObject : ICommandDataObject
    {
        TCommandDataObject DataObject { get; }
    }
}
