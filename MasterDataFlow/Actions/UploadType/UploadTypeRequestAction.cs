using System;

namespace MasterDataFlow.Actions.UploadType
{
    [Serializable]
    public class UploadTypeRequestAction : BaseAction
    {
        public const string ActionName = "UploadTypeRequestAction";

        public override string Name
        {
            get { return ActionName; }
        }

        public string TypeName { get; set; }
        public string WorkflowKey { get; set; }
    }
}
