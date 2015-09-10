using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MasterDataFlow.Assemblies;
using MasterDataFlow.Assembly.Interfaces;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Assembly
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
            //System.Reflection.Assembly assembly;
            //if (_assemblies.TryGetValue(args.Name, out assembly))
            //{
            //    return assembly;
            //}

            foreach (var assembly in _assemblies.Values)
            {
                if(assembly.FullName == args.Name)
                    return assembly;
            }

            return null;
        }

        //public void SetDomain(AppDomain domain)
        //{
        //    _domain = domain;
        //    _domain.AssemblyResolve += MyResolveEventHandler;
        //    _domain.ResourceResolve += Domain_ResourceResolve;
        //    _domain.AssemblyLoad += Domain_AssemblyLoad; ;
        //    _domain.TypeResolve += Domain_TypeResolve;
        //    _domain.UnhandledException += Domain_UnhandledException;
        //    _domain.ReflectionOnlyAssemblyResolve += Domain_ReflectionOnlyAssemblyResolve; ;
        //}

        public bool LoadAssembly(string assemblyName, byte[] bytes)
        {
            var assembly = AppDomain.CurrentDomain.Load(bytes);
            _assemblies.Add(assemblyName, assembly);
            return true;
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

        public string Execute(string commandType, string dataObject, string dataObjectType, string commandKey, out Type resultType)
        {
            var commandKeyInstance = (CommandKey) BaseKey.DeserializeKey(commandKey);
            var dataObjectInstance = Creator.CreateDataObjectInstance(dataObject, dataObjectType);
            var command = Creator.CreateCommandInstance(commandKeyInstance, commandType, dataObjectInstance);
            var commandResult = command.BaseExecute();
            resultType = commandResult.GetType();
            var result = Serialization.Serializator.Serialize(commandResult);
            return result;
        }


        //private System.Reflection.Assembly Domain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    Console.WriteLine("Domain_TypeResolve : " + args.Name);
        //    return System.Reflection.Assembly.Load(_bytes);
        //}

        //private void Domain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    Console.WriteLine("Domain_UnhandledException : " + e.ToString());
        //}

        //private System.Reflection.Assembly Domain_TypeResolve(object sender, ResolveEventArgs args)
        //{
        //    Console.WriteLine("Domain_TypeResolve : " + args.Name);
        //    return System.Reflection.Assembly.Load(_bytes);
        //}

        //private void Domain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        //{
        //    Console.WriteLine("Domain_AssemblyLoad : " + args.LoadedAssembly.FullName);
        //}

        //private System.Reflection.Assembly Domain_ResourceResolve(object sender, ResolveEventArgs args)
        //{
        //    Console.WriteLine("Domain_ResourceResolve : " + args.Name);
        //    return System.Reflection.Assembly.Load(_bytes);
        //}

        //private System.Reflection.Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
        //{
        //    Console.WriteLine("AssemblyResolve : " + args.Name);
        //    return System.Reflection.Assembly.Load(_bytes);
        //}
    }

}
