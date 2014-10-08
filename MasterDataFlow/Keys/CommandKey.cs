using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Keys
{
    public class CommandKey : ServiceKey
    {
        private readonly Guid _id;

        public CommandKey()
        {
            _id = Guid.NewGuid();
        }

        public CommandKey(Guid id)
        {
            _id = id;
        }

        public Guid Id
        {
            get { return _id; }
        }
    }
}
