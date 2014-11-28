using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Assemblies
{
    // http://msdn.microsoft.com/en-us/library/dd153782(v=vs.110).aspx
    // http://www.codeproject.com/Articles/453778/Loading-Assemblies-from-Anywhere-into-a-New-AppDom
    public class AssemblyLoader
    {
        private readonly Dictionary<BaseKey, Dictionary<string, Assembly>> _assemblies = new Dictionary<BaseKey, Dictionary<string, Assembly>>();

        //public class Loader : MarshalByRefObject
        //{
        //    public void LoadAssembly(byte[] bytes)
        //    {
        //        Assembly.Load(bytes);
        //    }

        //    public void LoadAssembly(string fileName)
        //    {
        //        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        //        {
        //            Console.WriteLine("Stage::AssemblyResolve - " + args.Name);
        //            return Assembly.LoadFrom(fileName);
        //        };

        //        Assembly.LoadFrom(fileName);
        //    }
        //}

        public void Load(BaseKey key, string assemblyName, byte[] bytes)
        {
            Dictionary<string, Assembly> keyAssemblies;
            if(!_assemblies.TryGetValue(key, out keyAssemblies))
            {
                keyAssemblies = new Dictionary<string, Assembly>();
                _assemblies.Add(key, keyAssemblies);
            }

            //var appDomain = AppDomain.CurrentDomain;

            //ResolveEventHandler appDomainOnAssemblyResolve = (object sender, ResolveEventArgs args) =>
            //{
            //    Console.WriteLine("AssemblyResolve : " + args.Name);
            //    return null;
            //};

            //appDomain.AssemblyResolve += appDomainOnAssemblyResolve;
            var assembly = Assembly.Load(bytes);
            //appDomain.AssemblyResolve -= appDomainOnAssemblyResolve;

            if (!keyAssemblies.ContainsKey(assemblyName))
            {
                keyAssemblies.Add(assemblyName, assembly);
            }
            else
            {
                keyAssemblies[assemblyName] = assembly;
            }

            // TODO for loading assemblies into another domain
            ////_appDomain.Load(bytes);
            //_loader = (Loader)_appDomain.CreateInstanceAndUnwrap(typeof(Loader).Assembly.FullName, typeof(Loader).FullName);
            ////_stage = (Stage)_appDomain.CreateInstanceFrom(typeof(Stage).Assembly.Location, typeof(Stage).FullName).Unwrap();
            ////_stage.SetDomain(_appDomain);
            //_loader.LoadAssembly(bytes);
        }

        public Type GetLoadedType(BaseKey key, string typeName)
        {
            Dictionary<string, Assembly> keyAssemblies;
            if (!_assemblies.TryGetValue(key, out keyAssemblies))
            {
                return null;
            }

            var parts = typeName.Split(',');
            var singleTypeName = parts[0];


            //var appDomain = AppDomain.CurrentDomain;

            //ResolveEventHandler appDomainOnAssemblyResolve = (object sender, ResolveEventArgs args) =>
            //{
            //    Console.WriteLine("AssemblyResolve : " + args.Name);
            //    return null;
            //};
            //appDomain.AssemblyResolve += appDomainOnAssemblyResolve;
            //try
            //{
                foreach (var assembly in keyAssemblies.Values)
                {
                    var result = assembly.GetType(singleTypeName);
                    if (result != null)
                    {
                        return result;
                    }
                }
                return null;
            //}
            //finally
            //{
            //    appDomain.AssemblyResolve -= appDomainOnAssemblyResolve;
            //}
        }

        public object CreateInstance(BaseKey key, Type type)
        {
            Dictionary<string, Assembly> keyAssemblies;
            if (!_assemblies.TryGetValue(key, out keyAssemblies))
            {
                return null;
            }

            //var appDomain = AppDomain.CurrentDomain;

            //ResolveEventHandler appDomainOnAssemblyResolve = (object sender, ResolveEventArgs args) =>
            //{
            //    Console.WriteLine("AssemblyResolve : " + args.Name);
            //    return null;
            //};
            //appDomain.AssemblyResolve += appDomainOnAssemblyResolve;
            //try
            //{
                foreach (var assembly in keyAssemblies.Values)
                {
                    var result = assembly.GetType(type.FullName);
                    if (result != null)
                    {
                        var instance = assembly.CreateInstance(type.FullName);
                        return instance;
                    }
                }
                return null;
            //}
            //finally
            //{
            //    appDomain.AssemblyResolve -= appDomainOnAssemblyResolve;
            //}
        }

        //private AppDomain BuildChildDomain(AppDomain parentDomain)
        //{
        //    var evidence = new Evidence(parentDomain.Evidence);
        //    var setup = parentDomain.SetupInformation;
        //    return AppDomain.CreateDomain("Command Domain", evidence, setup);
        //}

        //public void Dispose()
        //{
        //    if (_appDomain != null)
        //    {
        //        AppDomain.Unload(_appDomain);
        //    }
        //}
    }
}
