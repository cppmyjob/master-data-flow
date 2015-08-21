using System;

namespace MasterDataFlow.Actions.ClientGateKey
{
    [Serializable]
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
