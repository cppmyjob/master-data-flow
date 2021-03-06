﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using Newtonsoft.Json;

namespace MasterDataFlow.Messages
{
    [Serializable]
    public class StopCommandMessage : CommandMessage
    {
        private readonly ICommandDataObject _data;

        public StopCommandMessage(CommandKey key, ICommandDataObject data) : base(key)
        {
            _data = data;
        }

        public ICommandDataObject Data
        {
            get { return _data; }
        }
    }
}
