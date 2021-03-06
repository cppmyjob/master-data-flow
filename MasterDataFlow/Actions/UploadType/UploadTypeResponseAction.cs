﻿using System;

namespace MasterDataFlow.Actions.UploadType
{
    [Serializable]
    public class UploadTypeResponseAction : BaseAction
    {
        public const string ActionName = "UploadTypeResponseAction";

        public override string Name
        {
            get { return ActionName; }
        }

        public string TypeName { get; set; }

        public byte[] AssemblyData { get; set; }
        public string AssemblyName { get; set; }
        public string WorkflowKey { get; set; }
    }
}
