using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Network.Publishing
{
    public class SubscriptionRequestConfirmation
    {
        private SubscribeKey _subscribeKey;
        private SubscriptionRequest.Action _action;

        public SubscribeKey SubscribeKey { get { return _subscribeKey; }}
        public SubscriptionRequest.Action Action { get { return _action; }}

        public SubscriptionRequestConfirmation(SubscribeKey subscribeKey, SubscriptionRequest.Action action)
        {
            _subscribeKey = subscribeKey;
            _action = action;
        }
    }
}
