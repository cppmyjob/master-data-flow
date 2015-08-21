using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Keys
{
    [Serializable]
    public class ServiceKey : BaseKey
    {
        static ServiceKey()
        {
            AddKeyResolving("sk", typeof(ServiceKey));
        }

        private Guid _id;

        public ServiceKey()
        {
            _id = Guid.NewGuid();
        }

        public ServiceKey(Guid id)
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
