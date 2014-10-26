﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Exceptions
{
    [Serializable]
    public class MasterDataFlowException : Exception
    {
        protected MasterDataFlowException() : base()
        {
        }

        public MasterDataFlowException(string message)
            : base(message)
        {
        }
    }
}
