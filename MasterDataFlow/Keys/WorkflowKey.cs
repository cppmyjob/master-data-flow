using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Keys
{
    public class WorkflowKey : ServiceKey, INotificationReceiverKey
    {
        public WorkflowKey() : base()
        {
        }

        public WorkflowKey(Guid id) : base(id) 
        {
            
        }
    }
}
