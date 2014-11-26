using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Interfaces
{
    public interface IInstanceFactory
    {
        BaseCommand CreateCommandInstance(WorkflowKey workflowKey, CommandKey commandKey, Type type, ICommandDataObject commandDataObject);
        Type GetType(WorkflowKey workflowKey, string typeName);
    }
}
