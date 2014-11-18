using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;

namespace MasterDataFlow.Assemblies
{
    public class AssemblyLoader
    {
        private AppDomain _appDomain;

        public void Load(byte[] bytes)
        {
            if (_appDomain == null)
            {
                _appDomain = BuildChildDomain(AppDomain.CurrentDomain);
            }
            _appDomain.Load(bytes);
        }

        public Type GetLoadedType(string typeName)
        {
            if (_appDomain == null)
                return null;

            var parts = typeName.Split(',');
            var singleTypeName = parts[0];
            var assemblies = _appDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var result = assembly.GetType(singleTypeName);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private AppDomain BuildChildDomain(AppDomain parentDomain)
        {
            var evidence = new Evidence(parentDomain.Evidence);
            var setup = parentDomain.SetupInformation;
            return AppDomain.CreateDomain("Command Domain", evidence, setup);
        }
    }
}
