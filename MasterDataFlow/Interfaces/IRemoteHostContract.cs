using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Interfaces
{
    public interface IRemoteHostContract
    {
        void UploadAssembly(byte[] data);
        void Execute(string typeName, string dataObject);
    }
}
