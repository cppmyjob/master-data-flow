using System;
using System.Collections.Generic;
using System.Linq;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Network.Packets;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Network.Publishing.Subscribers
{
    public abstract class BaseSubscriber : EventLoopHub
    {
        private readonly ServiceKey _key = new ServiceKey();
        private readonly List<Subscription> _subscriptions = new List<Subscription>(); 

        private readonly AsyncQueue<EventMessage> _receivedMessages = new AsyncQueue<EventMessage>(); 

        private readonly object _lockObject = new object();

        public override BaseKey Key
        {
            get { return _key; }
        }

        public virtual SubscribeKey Subscribe(SubscriptionEndpoint endpoint)
        {
            var subscription = new Subscription(endpoint, Key);
            var subscriptionRequest = new SubscriptionRequest(subscription, SubscriptionRequest.Action.Subscribe);
            var packet = new Packet(Key, endpoint.PublisherKey, subscriptionRequest);
            Send(packet);

            lock (_lockObject)
            {
                _subscriptions.Add(subscription);
            }

            return subscription.Key;
        }

        public virtual void Unsubscribe(SubscribeKey key)
        {
            var subscription = _subscriptions.FirstOrDefault(s => s.Key == key);
            if (subscription == null)
                return;

            var subscriptionRequest = new SubscriptionRequest(subscription, SubscriptionRequest.Action.Unsubscribe);
            var packet = new Packet(Key, subscription.Endpoint.PublisherKey, subscriptionRequest);
            Send(packet);

            lock (_lockObject)
            {
                subscription.Deactivate();
            }
        }

        protected virtual void ConfirmSubscription(SubscribeKey key)
        {
            var subscribe = _subscriptions.FirstOrDefault(s => s.Key == key);
            if (subscribe != null)
                subscribe.Activate();
        }

        protected virtual void ConfirmUnsubscription(SubscribeKey key)
        {
            var subscribe = _subscriptions.FirstOrDefault(s => s.Key == key);
            if (subscribe != null)
            {
                lock (_lockObject)
                {
                    _subscriptions.Remove(subscribe);
                }
            }
        }

        protected override void ProccessPacket(IPacket packet)
        {
            var message = packet.Body as EventMessage;
            if (message != null)
            {
                _receivedMessages.Enqueue(message);
                return;
            }

            var confirmation = packet.Body as SubscriptionRequestConfirmation;
            if (confirmation != null)
            {
                switch (confirmation.Action)
                {
                    case SubscriptionRequest.Action.Subscribe:
                        ConfirmSubscription(confirmation.SubscribeKey);
                        break;
                    case SubscriptionRequest.Action.Unsubscribe:
                        ConfirmUnsubscription(confirmation.SubscribeKey);
                        break;
                }
            }

        }
    }
}
