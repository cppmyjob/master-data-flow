using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Network.Publishing
{
    public class Subscription
    {
        private readonly SubscribeKey _key = new SubscribeKey();
        private readonly BaseKey _subscriberKey;
        private readonly SubscriptionEndpoint _endpoint;

        public SubscribeKey Key { get { return _key; }}
        public BaseKey SubscriberKey { get { return _subscriberKey; }}
        public SubscriptionEndpoint Endpoint { get { return _endpoint; }}

        public bool Active { get; private set; }

        public Subscription(SubscriptionEndpoint endpoint, BaseKey subscriberKey)
        {
            _endpoint = endpoint;
            _subscriberKey = subscriberKey;
        }

        public void Activate()
        {
            Active = true;
        }

        public void Deactivate()
        {
            Active = false;
        }
    }
}
