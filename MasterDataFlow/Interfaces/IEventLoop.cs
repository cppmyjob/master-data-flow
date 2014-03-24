using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Interfaces
{
    internal interface IEventLoop
    {
        void Push(BaseCommand command);
    }
}
