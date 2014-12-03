using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Network.Publishing
{
    public class SubscriptionEndpoint
    {
        private readonly Type _event;
        private readonly BaseKey _publisherKey;

        public Type EventType { get { return _event; } }
        public BaseKey PublisherKey { get { return _publisherKey; } }

        public SubscriptionEndpoint(Type eventInstance, BaseKey publisherKey)
        {
            _event = eventInstance;
            _publisherKey = publisherKey;
        }
    }
}
