using System;

namespace MasterDataFlow.Interfaces
{
    public interface ILoader
    {
        void LoadAssembly(string assemblyName, byte[] bytes);

        Type GetLoadedType(string typeName);

        bool IsTypeExists(string typeName);

        string[] GetDomainAssemblies();

        string Execute(string commandType, string dataObject, string dataObjectType, string workflowKey, 
            string commandKey, out Type resultType, IMessageSender messageSender);
    }
}
