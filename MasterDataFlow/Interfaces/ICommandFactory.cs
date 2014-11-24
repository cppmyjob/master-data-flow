using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Interfaces
{
    public interface ICommandFactory
    {
        BaseCommand CreateInstance(WorkflowKey workflowKey, CommandKey commandKey, Type type, ICommandDataObject commandDataObject);
    }
}
