using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.EventLoop
{
    public enum EventLoopCommandStatus
    {
        NotStarted = 0,
        Progress = 1,
        Completed = 2,
        Fault = 3
    }
}
