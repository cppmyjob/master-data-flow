using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using MasterDataFlow.Actions;
using MasterDataFlow.Assembly.Interfaces;
using MasterDataFlow.Keys;
using Newtonsoft.Json;

namespace MasterDataFlow.Assemblies
{
    // http://msdn.microsoft.com/en-us/library/dd153782(v=vs.110).aspx
    // http://www.codeproject.com/Articles/453778/Loading-Assemblies-from-Anywhere-into-a-New-AppDom
    public class AssemblyLoader
    {
        private class InternalDomain
        {
            public AppDomain Domain { get; set; }

            public ILoader Loader { get; set; }
        }

        private readonly Dictionary<BaseKey, InternalDomain> _domains = new Dictionary<BaseKey, InternalDomain>();

        public AssemblyLoader()
        {
            //AppDomain.CurrentDomain.AssemblyResolve += MyResolveEventHandler;
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
                domain.Loader =(ILoader) domain.Domain.CreateInstanceAndUnwrap("MasterDataFlow.Assembly", "MasterDataFlow.Assembly.Loader");
                _domains.Add(key, domain);
            }

            //var appDomain = AppDomain.CurrentDomain;

            //domain.Domain.AssemblyResolve += MyResolveEventHandler;
            //domain.Domain.ResourceResolve += Domain_ResourceResolve;
            //domain.Domain.AssemblyLoad += Domain_AssemblyLoad; ;
            //domain.Domain.TypeResolve += Domain_TypeResolve;
            //domain.Domain.UnhandledException += Domain_UnhandledException;
            //domain.Domain.ReflectionOnlyAssemblyResolve += Domain_ReflectionOnlyAssemblyResolve; 

            var assembly = domain.Loader.LoadAssembly(assemblyName, bytes);
            //domain.Domain.AssemblyResolve -= appDomainOnAssemblyResolve;

            // TODO for loading assemblies into another domain
            ////_appDomain.Load(bytes);
            //_loader = (Loader)_appDomain.CreateInstanceAndUnwrap(typeof(Loader).Assembly.FullName, typeof(Loader).FullName);
            ////_stage = (Stage)_appDomain.CreateInstanceFrom(typeof(Stage).Assembly.Location, typeof(Stage).FullName).Unwrap();
            ////_stage.SetDomain(_appDomain);
            //_loader.LoadAssembly(bytes);
        }

        //private static Assembly Domain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    Console.WriteLine("Domain_TypeResolve : " + args.Name);
        //    return null;
        //}

        //private static void Domain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    Console.WriteLine("Domain_UnhandledException : " + e.ToString());
        //}

        //private static Assembly Domain_TypeResolve(object sender, ResolveEventArgs args)
        //{
        //    Console.WriteLine("Domain_TypeResolve : " + args.Name);
        //    return null;
        //}

        private static void Domain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Console.WriteLine("Domain_AssemblyLoad : " + args.LoadedAssembly.FullName);
        }

        //private static Assembly Domain_ResourceResolve(object sender, ResolveEventArgs args)
        //{
        //    Console.WriteLine("Domain_ResourceResolve : " + args.Name);
        //    return null;
        //}

        //private static System.Reflection.Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
        //{
        //    Console.WriteLine("AssemblyResolve : " + args.Name);
        //    return null;
        //}

        //public Type GetLoadedType(BaseKey key, string typeName)
        //{
        //    InternalDomain domain;
        //    if (!_domains.TryGetValue(key, out domain))
        //    {
        //        return null;
        //    }

        //    return domain.Loader.GetLoadedType(typeName);
        //}

        public bool IsTypeExists(BaseKey key, string typeName)
        {
            InternalDomain domain;
            if (!_domains.TryGetValue(key, out domain))
            {
                return false;
            }

            return domain.Loader.IsTypeExists(typeName);
        }


        public string PrintLoadedTypes(BaseKey key)
        {
            InternalDomain domain;
            if (!_domains.TryGetValue(key, out domain))
            {
                return null;
            }
            var result = new StringBuilder();

            result.AppendLine(string.Format("Loaded assemblies in appdomain: {0}", domain.Domain.FriendlyName));
            var names = domain.Domain.GetAssemblies().Select(t => t.GetName().Name).OrderBy(t => t);
            foreach (var name in names)
            {
                result.AppendLine(string.Format("- {0}", name));
            }
            return result.ToString();
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


    }
}
