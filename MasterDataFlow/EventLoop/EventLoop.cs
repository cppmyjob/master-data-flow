using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MasterDataFlow.EventLoop
{
    // TODO now it's wrapper. but we are planning to create a single thread handler of all events
    public class EventLoop
    {
        public static void QueueEvent(WaitCallback callback)
        {
            ThreadPool.QueueUserWorkItem(callback);
        }
    }
}
