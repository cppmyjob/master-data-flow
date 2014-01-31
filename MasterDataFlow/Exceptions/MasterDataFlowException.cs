using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Exceptions
{
    public class MasterDataFlowException : Exception
    {
        public MasterDataFlowException(string message)
            : base(message)
        {
        }
    }
}
