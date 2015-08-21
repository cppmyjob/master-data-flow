using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Keys
{
    [Serializable]
    public class SubscribeKey : BaseKey
    {
        static SubscribeKey()
        {
            AddKeyResolving("subscribe_key", typeof(SubscribeKey));
        }

        private Guid _id;

        public SubscribeKey()
        {
            _id = Guid.NewGuid();
        }

        public SubscribeKey(Guid id)
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
