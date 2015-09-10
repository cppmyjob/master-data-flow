using System;
using System.IO;
using System.Linq;
using System.Reflection;
using MasterDataFlow.Assemblies;
using MasterDataFlow.Keys;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Tests
{
    // http://msdn.microsoft.com/en-us/library/dd153782(v=vs.110).aspx
    [TestClass]
    public class AssemblyLoaderTests
    {
        private const string FirstExternalAssembly = @"External/First/MasterDataFlow.Tests.ExternalAssembly.dll";
        private const string SecondExternalAssembly = @"External/Second/MasterDataFlow.Tests.ExternalAssembly.dll";

        private AssemblyLoader _loader;

        [TestInitialize]
        public void TestInitialize()
        {
            _loader = new AssemblyLoader();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            //_loader.Dispose();
        }

        [TestMethod]
        public void IsClassExistsTest()
        {
            // ARRANGE
            var bytes = LoadFirstExternalAssembly();
            var workflowKey = new WorkflowKey();
            var assemblyName = "MasterDataFlow.Tests.ExternalAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            _loader.Load(workflowKey, assemblyName, bytes);
                
            // ACT
            var classType = _loader.IsTypeExists(workflowKey, "MasterDataFlow.Tests.ExternalAssembly.MathCommand, MasterDataFlow.Tests.ExternalAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            Console.WriteLine(_loader.PrintLoadedTypes(workflowKey));

            // ASSERT
            Assert.IsNotNull(classType);
        }

        //[TestMethod]
        //public void CreateInstanceTest()
        //{
        //    // ARRANGE
        //    var bytes = LoadFirstExternalAssembly();
        //    _loader.Load(bytes);

        //    // ACT
        //    var classType = _loader.GetLoadedType("MasterDataFlow.Tests.ExternalAssembly.MathCommand, MasterDataFlow.Tests.ExternalAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
        //    LogLoadedAssemblies();

        //    // ASSERT
        //    Assert.IsNotNull(classType);
        //}

        private static void LogLoadedAssemblies()
        {
            LogLoadedAssemblies(AppDomain.CurrentDomain);
        }

        private static void LogLoadedAssemblies(AppDomain appDomain)
        {
            Console.WriteLine("Loaded assemblies in appdomain: {0}", appDomain.FriendlyName);
            var names = AppDomain.CurrentDomain.GetAssemblies().Select(t => t.GetName().Name).OrderBy(t => t);
            foreach (var name in names)
            {
                Console.WriteLine("- {0}", name);
            }
        }

        private byte[] LoadFirstExternalAssembly()
        {
            var result = File.ReadAllBytes(FirstExternalAssembly);
            return result;
        }
    }
}
