using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Network
{
    public abstract class Hub : IHub
    {
        private readonly AsyncDictionary<BaseKey, IHub> _connectedHubs = new AsyncDictionary<BaseKey, IHub>();
        private readonly AsyncQueue<IPacket> _queue = new AsyncQueue<IPacket>();

        public abstract BaseKey Key { get; }

        public virtual bool ConnectHub(IHub hub)
        {
            // TODO check if exists
            _connectedHubs.AddItem(hub.Key, hub);
            // TODO handle not support cases
            hub.AcceptHub(this);
            return true;
        }

        public virtual void AcceptHub(IHub hub)
        {
            throw new NotSupportedException();
        }

        public virtual void DisconnectHub(IHub hub)
        {
            throw new NotImplementedException();
        }

        public virtual SendStatus Send(IPacket packet)
        {
            _queue.Enqueue(packet);
            return SendStatus.Ok;
        }

        protected abstract void ProccessPacket(IPacket packet);

        protected internal bool Loop()
        {
            if (_queue.Count == 0)
                return false;
            var packet = _queue.Dequeue();

            if (packet.RecieverKey == Key)
            {
                ProccessPacket(packet);
                return true;
            }
            var reciever = _connectedHubs.GetItem(packet.RecieverKey);
            if (reciever != null)
            {
                reciever.Send(packet);
            }
            else
            {
                ProcessUndeliveredPacket(packet);
            }
            return true;
        }

        protected virtual void ProcessUndeliveredPacket(IPacket packet)
        {
            
        }
    }
}
