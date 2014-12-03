using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Messages
{
    public class EventMessage : BaseMessage
    {
        private readonly Type _occuredEventType;

        private readonly object _body;

        public Type OccuredEventKey { get { return _occuredEventType; }}
        public object Body { get { return _body; }}

        public EventMessage(Type occuredEventKey, object body)
        {
            _occuredEventType = occuredEventKey;
            _body = body;
        }
    }
}
