using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Actions
{
    public class RemoteExecuteCommandAction : BaseAction
    {
        public class Info
        {
            public string CommandType { get; set; }
            public string DataObject { get; set; }
            public string DataObjectType { get; set; }
            public string WorkflowKey { get; set; }
            public string CommandKey { get; set; }        
        }

        public const string ActionName = "RemoteExecuteCommandAction";

        public Info CommandInfo { get; set; }

        public override string Name
        {
            get { return ActionName; }
        }
    }
}
