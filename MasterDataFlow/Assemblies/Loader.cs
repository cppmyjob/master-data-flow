using System;
using System.Collections.Generic;
using System.Linq;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Assemblies
{
    public class Loader : MarshalByRefObject, ILoader
    {
        private readonly Dictionary<string, System.Reflection.Assembly> _assemblies = new Dictionary<string, System.Reflection.Assembly>();

        public Loader()
        {
            var ck = new ServiceKey();
            var wfk = new WorkflowKey();
            var cmk = new CommandKey();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // TODO improve performance
            foreach (var assembly in _assemblies.Values)
            {
                if(assembly.FullName == args.Name)
                    return assembly;
            }

            return null;
        }

        public void LoadAssembly(string assemblyName, byte[] bytes)
        {
            var assembly = AppDomain.CurrentDomain.Load(bytes);
            _assemblies.Add(assemblyName, assembly);
        }

        public Type GetLoadedType(string typeName)
        {
            var parts = typeName.Split(',');
            var singleTypeName = parts[0];

            foreach (var assembly in _assemblies.Values)
            {
                var result = assembly.GetType(singleTypeName);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public bool IsTypeExists(string typeName)
        {
            var parts = typeName.Split(',');
            var singleTypeName = parts[0];

            return _assemblies.Values.Select(assembly => assembly.GetType(singleTypeName)).Any(result => result != null);
        }

        public string[] GetDomainAssemblies()
        {
            var result = AppDomain.CurrentDomain.GetAssemblies().Select(t => t.GetName().FullName).OrderBy(t => t).ToArray();
            return result;
        }

        public string Execute(string commandType, string dataObject, string dataObjectType, string workflowKey, string commandKey, 
            out Type resultType, IMessageSender messageSender)
        {
            var commandKeyInstance = (CommandKey)BaseKey.DeserializeKey(commandKey);
            var workflowKeyInstance = (WorkflowKey)BaseKey.DeserializeKey(workflowKey);
            var dataObjectInstance = Creator.CreateDataObjectInstance(dataObject, dataObjectType);
            var command = Creator.CreateCommandInstance(workflowKeyInstance, commandKeyInstance, commandType, dataObjectInstance, messageSender);
            var commandResult = command.BaseExecute();
            resultType = commandResult.GetType();
            var result = Serialization.Serializator.Serialize(commandResult);
            return result;
        }

    }

}
