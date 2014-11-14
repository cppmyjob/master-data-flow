namespace MasterDataFlow.Actions.UploadType
{
    public class UploadTypeRequestAction : BaseAction
    {
        public const string ActionName = "UploadTypeRequestAction";

        public override string Name
        {
            get { return ActionName; }
        }

        public string TypeName { get; set; }
    }
}
