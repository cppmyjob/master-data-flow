using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;

namespace MasterDataFlow.Interfaces
{
    public interface IMessageSender
    {
        void Send(BaseKey recipient, CommandMessage message);
    }
}
