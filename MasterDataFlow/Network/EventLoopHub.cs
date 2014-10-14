using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces.Network;

namespace MasterDataFlow.Network
{
    public abstract class EventLoopHub : Hub
    {
        public override SendStatus Send(IPacket packet)
        {
            var result = base.Send(packet);
            EventLoop.EventLoop.QueueEvent(state => Loop());
            return result;
        }
    }
}
