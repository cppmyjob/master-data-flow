﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Keys
{
    public class CommandKey : ServiceKey
    {
        public CommandKey() : base()
        {
        }

        public CommandKey(Guid id)
            : base(id) 
        {
            
        }
    }
}