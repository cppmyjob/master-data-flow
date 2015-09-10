using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Assembly.Interfaces
{
    public interface ILoader
    {
        bool LoadAssembly(string assemblyName, byte[] bytes);

        Type GetLoadedType(string typeName);

        bool IsTypeExists(string typeName);

        string Execute(string commandType, string dataObject, string dataObjectType, string commandKey, out Type resultType);
    }
}
