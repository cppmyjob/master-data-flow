using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Actions
{
    public class LocalExecuteCommandAction : BaseAction
    {
        public const string ActionName = "LocalExecuteCommandAction";

        public CommandInfo CommandInfo { get; internal set; }

        public override string Name
        {
            get { return ActionName; }
        }
    }
}
