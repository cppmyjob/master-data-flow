using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Interfaces
{
    public interface IRemoteHostContract
    {
        void UploadAssembly(byte[] data);
        void Execute(WorkflowKey workflowKey, CommandKey commandKey, string commandTypeName, string dataObjectTypeName, string dataObject);
    }
}
