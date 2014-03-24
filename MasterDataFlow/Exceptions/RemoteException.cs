using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Exceptions
{
    public class RemoteException : MasterDataFlowException
    {
        public RemoteException(string message)
            : base(message)
        {
        }
    }
}
