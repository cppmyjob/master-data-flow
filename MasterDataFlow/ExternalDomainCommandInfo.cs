using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Assemblies;
using MasterDataFlow.Keys;

namespace MasterDataFlow
{
    public class ExternalDomainCommandInfo
    {
        public string CommandType { get; set; }
        public string DataObject { get; set; }
        public string DataObjectType { get; set; }
        public WorkflowKey WorkflowKey { get; set; }
        public string CommandKey { get; set; }
        public AssemblyLoader AssemblyLoader { get; set; }

    }
}
