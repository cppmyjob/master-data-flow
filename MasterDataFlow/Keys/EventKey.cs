using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Keys
{
    [Serializable]
    public class EventKey : BaseKey
    {
        static EventKey()
        {
            AddKeyResolving("event_key", typeof(EventKey));
        }

        private Guid _id;

        public EventKey()
        {
            _id = Guid.NewGuid();
        }

        public EventKey(Guid id)
        {
            _id = id;
        }

        public Guid Id
        {
            get { return _id; }
            private set { _id = value; }
        }
    }
}
