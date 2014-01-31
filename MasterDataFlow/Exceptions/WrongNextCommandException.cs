using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Exceptions
{
    public class WrongNextCommandException : MasterDataFlowException
    {
        public WrongNextCommandException(string message)
            : base(message)
        {
        }

    }
}
