using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Network.Publishing
{
    public class SubscriptionRequest
    {
        public enum Action
        {
            Subscribe,
            Unsubscribe
        }

        private readonly Subscription _subscription;
        private readonly Action _subscribeAction;

        public Subscription Subscription { get { return _subscription; } }
        public Action SubscribeAction { get { return _subscribeAction; }}

        public SubscriptionRequest(Subscription subscription, Action action)
        {
            _subscription = subscription;
            _subscribeAction = action;
        }
    }
}
