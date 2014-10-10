using System;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Utils;

namespace MasterDataFlow.EventLoop
{
    public delegate void EventLoopCallback(INotificationReceiverKey key, object notificationData);

    public class BaseEventLoop
    {
        internal class LoopItem
        {
            public LoopItem(INotificationReceiverKey key)
            {
                Key = key;
            }

            public INotificationReceiverKey Key { get; set; }
            public ILoopCommand Command { get; set; }
        }


        private readonly AsyncQueue<LoopItem> _queue = new AsyncQueue<LoopItem>();

        internal AsyncQueue<LoopItem> Queue
        {
            get { return _queue; }
        }

        protected internal void Push(INotificationReceiverKey key, ILoopCommand command)
        {
            var item = new LoopItem(key)
            {
                Command = command,
            };
            _queue.Enqueue(item);
        }

        protected internal bool Loop()
        {
            if (_queue.Count == 0)
                return false;
            var item = _queue.Dequeue();
            if (!item.Command.Execute(item.Key, Notify))
                _queue.Enqueue(item);
            return true;
        }

        protected internal void Notify(INotificationReceiverKey key, object notificationData)
        {
            
        }

    }
}