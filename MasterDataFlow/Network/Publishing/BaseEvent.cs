using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;

namespace MasterDataFlow.Network.Publishing
{
    public abstract class BaseEvent
    {
        private readonly EventKey _key = new EventKey();

        public EventKey Key { get { return _key; }}

        public delegate void BaseEventHandler(EventMessage message);

        private readonly BaseEventHandler _handler;

        public BaseEventHandler Handler
        {
            get { return _handler; }
        }

        protected BaseEvent(BaseEventHandler handler)
        {
            _handler = handler;
        }

        protected virtual EventMessage BuildMessage()
        {
            return new EventMessage(GetType(), null);
        }

        protected virtual void EventRaise()
        {
            EventMessage message = BuildMessage();
            _handler(message);
        }

    }
}
