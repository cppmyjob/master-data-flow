using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Keys
{
    [Serializable]
    public class CommandKey : ServiceKey
    {
        static CommandKey()
        {
            AddKeyResolving("cmk", typeof(CommandKey));
        }

        public CommandKey() : base()
        {
        }

        public CommandKey(Guid id)
            : base(id) 
        {
            
        }
    }
}
