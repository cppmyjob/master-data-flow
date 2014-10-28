using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Actions
{
    public class UploadTypeAction : BaseAction
    {
        public const string ActionName = "UploadTypeAction";

        public override string Name
        {
            get { return ActionName; }
        }

        public string UploadType { get; set; }
    }
}
