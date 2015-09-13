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
            _loader.Dispose();
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
            var isTypeExists = _loader.IsTypeExists(workflowKey, "MasterDataFlow.Tests.ExternalAssembly.MathCommand, MasterDataFlow.Tests.ExternalAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

            // ASSERT
            Assert.IsNotNull(isTypeExists);
        }

        [TestMethod]
        public void AssemlyIsLoadedIntoExternalDomainOnly()
        {
            // ARRANGE
            var bytes = LoadFirstExternalAssembly();
            var workflowKey = new WorkflowKey();
            var assemblyName = "MasterDataFlow.Tests.ExternalAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            _loader.Load(workflowKey, assemblyName, bytes);

            // ACT
            var assemblies = _loader.GetDomainAssemblies(workflowKey);

            // ASSERT
            Assert.IsTrue(assemblies.Any(t => t == assemblyName));
            Assert.AreEqual(1, assemblies.Count(t => t == assemblyName));
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies().Select(t => t.GetName().FullName).ToArray();
            Assert.IsFalse(currentAssemblies.Any(t => t == assemblyName));
        }

        private byte[] LoadFirstExternalAssembly()
        {
            var result = File.ReadAllBytes(FirstExternalAssembly);
            return result;
        }
    }
}
