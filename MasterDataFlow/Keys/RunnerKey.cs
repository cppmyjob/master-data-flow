using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Keys
{
    public class RunnerKey : ServiceKey, INotificationReceiverKey
    {
        public RunnerKey() : base()
        {
        }

        public RunnerKey(Guid id)
            : base(id) 
        {
            
        }
    }
}
