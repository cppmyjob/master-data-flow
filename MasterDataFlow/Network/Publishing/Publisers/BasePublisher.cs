using System.Collections.Generic;
using System.Linq;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Network.Packets;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Network.Publishing.Publisers
{
    public abstract  class BasePublisher : EventLoopHub
    {
        private readonly List<Subscription> _subscriptions = new List<Subscription>();

        //protected readonly List<BaseEvent> Events = new List<BaseEvent>();
 
        private readonly AsyncQueue<EventMessage> _messageQueue = new AsyncQueue<EventMessage>();

        protected override void ProccessPacket(IPacket packet)
        {
            var subscriptionRequest = packet.Body as SubscriptionRequest;
            if (subscriptionRequest != null)
            {
                switch (subscriptionRequest.SubscribeAction)
                {
                    case SubscriptionRequest.Action.Subscribe:
                        if (AcceptSubscription(subscriptionRequest.Subscription))
                        {
                            SendConfirmationToSubscriber(subscriptionRequest, packet.SenderKey);
                        }
                        break;
                    case SubscriptionRequest.Action.Unsubscribe:
                        if (RemoveSubscription(subscriptionRequest.Subscription))
                        {
                            SendConfirmationToSubscriber(subscriptionRequest, packet.SenderKey);
                        }
                        break;
                }
            }
        }

        protected void SendConfirmationToSubscriber(SubscriptionRequest request, BaseKey receiverKey)
        {
            SubscriptionRequestConfirmation confirmation = new SubscriptionRequestConfirmation(request.Subscription.Key, request.SubscribeAction);

            Packet confirmationPacket = new Packet(Key, receiverKey, confirmation);
            Send(confirmationPacket);
        }

        protected virtual bool AcceptSubscription(Subscription subscription)
        {
            if (
                _subscriptions.Any(
                    s =>
                        s.Key == subscription.Key ||
                        (s.Endpoint.EventType == subscription.Endpoint.EventType && s.SubscriberKey == subscription.SubscriberKey)))
                return false;
            _subscriptions.Add(subscription);
            return true;
        }

        protected virtual bool RemoveSubscription(Subscription subscription)
        {
            var subscriptionToRemove = _subscriptions.FirstOrDefault(s => s.Key == subscription.Key);
            if (subscriptionToRemove != null)
            {
                _subscriptions.Remove(subscriptionToRemove);
                return true;
            }
            return false;
        }

        protected virtual void Publish()
        {
            if (_messageQueue.Count == 0)
                return;

            var message = _messageQueue.Dequeue();

            var subscribtions = _subscriptions.Where(s => s.Endpoint.EventType == message.OccuredEventKey).ToList();

            foreach (var packet in subscribtions.Select(subscribtion => new Packet(Key, subscribtion.SubscriberKey, message)))
            {
                Send(packet);
            }
        }

        protected virtual void PublisherEventHandler(EventMessage message)
        {
            _messageQueue.Enqueue(message);
            EventLoop.EventLoop.QueueEvent(state => Publish());
        }

        //public List<BaseEvent> GetAvailableEvents()
        //{
        //    List<BaseEvent> deepCopyList = new List<BaseEvent>();
        //    foreach (var baseEvent in Events)
        //        deepCopyList.Add(baseEvent);
        //    return deepCopyList;
        //}
    }
}
