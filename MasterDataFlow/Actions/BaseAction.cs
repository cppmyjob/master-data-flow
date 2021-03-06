﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MasterDataFlow.Actions
{
    [Serializable]
    public abstract class BaseAction
    {
        [JsonIgnore]
        public abstract string Name { get; }
    }
}
