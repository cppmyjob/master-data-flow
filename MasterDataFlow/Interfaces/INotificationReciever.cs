using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Interfaces
{
    public interface INotificationReciever
    {
        INotificationReceiverKey RecieverKey { get; }
        void Notify(object data);
    }
}
