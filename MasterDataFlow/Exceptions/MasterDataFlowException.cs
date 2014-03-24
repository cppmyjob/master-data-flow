using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Exceptions
{
    public abstract class MasterDataFlowException : Exception
    {
        protected MasterDataFlowException() : base()
        {
        }

        protected MasterDataFlowException(string message)
            : base(message)
        {
        }
    }
}
