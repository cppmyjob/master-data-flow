using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Actions
{
    public class FindContainerAndLaunchCommandAction : BaseAction
    {
        // TODO getting action Name in runtime
        public const string ActionName = "FindContainerAndLaunchCommandAction";

        public LocalDomainCommandInfo LocalDomainCommandInfo { get; internal set; }
        public ExternalDomainCommandInfo ExternalDomainCommandInfo { get; internal set; }
        
        public override string Name
        {
            get { return ActionName; }
        }
    }
}
