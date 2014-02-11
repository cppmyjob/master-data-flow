using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Tests.Mocks
{
    public class MockRemoteHostContract : IRemoteHostContract
    {
        public void UploadAssembly(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void Execute(string typeName, string dataObject)
        {
            throw new NotImplementedException();
        }
    }
}
