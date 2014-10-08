using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Keys
{
    public class WorkflowKey : ServiceKey
    {
        private readonly Guid _id;

        public WorkflowKey()
        {
            _id = Guid.NewGuid();
        }

        public WorkflowKey(Guid id)
        {
            _id = id;
        }

        public Guid Id
        {
            get { return _id; }
        }
    }
}
