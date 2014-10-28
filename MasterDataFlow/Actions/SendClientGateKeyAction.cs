using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MasterDataFlow.Actions
{
    public class SendClientGateKeyAction : BaseAction
    {
        public const string ActionName = "SendClientGateKeyAction";

        public override string Name
        {
            get { return ActionName; }
        }

        public string ClientGateKey { get; set; }
    }
}
