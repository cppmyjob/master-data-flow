using System;

namespace MasterDataFlow.Actions.ClientGateKey
{
    [Serializable]
    public class ClientGateKeyRecievedAction: BaseAction
    {
        public const string ActionName = "ClientGateKeyRecievedAction";

        public override string Name
        {
            get { return ActionName; }
        }
    }
}
