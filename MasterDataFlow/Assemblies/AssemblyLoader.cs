using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using MasterDataFlow.Actions;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using Newtonsoft.Json;

namespace MasterDataFlow.Assemblies
{
    // http://msdn.microsoft.com/en-us/library/dd153782(v=vs.110).aspx
    // http://www.codeproject.com/Articles/453778/Loading-Assemblies-from-Anywhere-into-a-New-AppDom
    // TODO Thread Safe
    public class AssemblyLoader : IDisposable
    {
        private class InternalDomain
        {
            public AppDomain Domain { get; set; }

            public ILoader Loader { get; set; }
        }

        private readonly Dictionary<BaseKey, InternalDomain> _domains = new Dictionary<BaseKey, InternalDomain>();

        public AssemblyLoader()
        {
        }

        public void Load(BaseKey key, string assemblyName, byte[] bytes)
        {
            InternalDomain domain;
            if(!_domains.TryGetValue(key, out domain))
            {
                var evidence = new Evidence(AppDomain.CurrentDomain.Evidence);
                AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
                domain = new InternalDomain
                {
                    Domain = AppDomain.CreateDomain(key.Key, evidence, setup),
                };
                // TODO dynamic assembly and class name
                domain.Loader =(ILoader) domain.Domain.CreateInstanceAndUnwrap("MasterDataFlow", "MasterDataFlow.Assemblies.Loader");
                _domains.Add(key, domain);
            }

            domain.Loader.LoadAssembly(assemblyName, bytes);
        }

        public bool IsTypeExists(BaseKey key, string typeName)
        {
            InternalDomain domain;
            if (!_domains.TryGetValue(key, out domain))
            {
                return false;
            }

            return domain.Loader.IsTypeExists(typeName);
        }


        public string[] GetDomainAssemblies(BaseKey key)
        {
            InternalDomain domain;
            if (!_domains.TryGetValue(key, out domain))
            {
                return null;
            }
            return domain.Loader.GetDomainAssemblies();
        }

        public string LocalExecuteCommandAction(WorkflowKey workflowKey,string commandType, string dataObject, string dataObjectType, string commandKey, out Type resultType)
        {
            InternalDomain domain;
            if (!_domains.TryGetValue(workflowKey, out domain))
            {
                resultType = null;
                // TODO Send Error Message?
                return null;
            }

            var result = domain.Loader.Execute(commandType, dataObject, dataObjectType, commandKey, out resultType);
            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var domain in _domains)
                {
                    AppDomain.Unload(domain.Value.Domain);
                }
            }
        }
    }
}
