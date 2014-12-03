using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Messages;

namespace MasterDataFlow.Interfaces
{
    public interface IBaseEvent
    {
        EventMessage BuildMessage(object eventInfo);
    }
}
