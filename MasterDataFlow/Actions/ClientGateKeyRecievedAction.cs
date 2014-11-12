using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Actions
{
    public class ClientGateKeyRecievedAction: BaseAction
    {
        public const string ActionName = "ClientGateKeyRecievedAction";

        public override string Name
        {
            get { return ActionName; }
        }
    }
}
