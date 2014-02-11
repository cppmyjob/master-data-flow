using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Results;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class CommandDomainInstanceTests
    {
        private CommandDomainInstance _сommandDomainInstance;
        private CommandDomain _сommandDomain;

        [TestInitialize]
        public void TestInitialize()
        {
            _сommandDomain = new CommandDomain();
            _сommandDomainInstance = new CommandDomainInstance(_сommandDomain);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _сommandDomainInstance.Dispose();
        }

        [TestMethod]
        public void ExecuteCompletedTest()
        {
            // ARRANGE
            _сommandDomainInstance.AddContainter(new SimpleContainer());

            var definition = CommandBuilder.Build<CommandStub>().Complete();
            _сommandDomain.Register(definition);

            // ACT
            var context = _сommandDomainInstance.Start<CommandStub>(new CommandDataObjectStub());
            WaitHandle.WaitAll(new[] { context.GetWaiter() }, 1000);

            // ASSERT
            Assert.AreEqual(ExecuteStatus.Completed, context.Status);
            Assert.IsNull(context.Result);
            Assert.IsNull(context.Exception);
        }

        [TestMethod]
        public void FinalResultTest()
        {
            // ARRANGE
            _сommandDomainInstance.AddContainter(new SimpleContainer());
            var definition = CommandBuilder.Build<CommandStub>().Complete();
            _сommandDomain.Register(definition);

            // ACT
            var context = _сommandDomainInstance.Start<CommandStub>(new CommandDataObjectStub());
            WaitHandle.WaitAll(new[] { context.GetWaiter() }, 1000);

            // ASSERT
            Assert.IsNull(context.Result);
        }


        [TestMethod]
        public void PassingInputDataToResultTest()
        {
            // ARRANGE
            _сommandDomainInstance.AddContainter(new SimpleContainer());
            var definition = CommandBuilder.Build<PassingCommand>().Complete();
            _сommandDomain.Register(definition);

            // ACT
            var id = Guid.NewGuid();
            var context = _сommandDomainInstance.Start<PassingCommand>(new PassingCommandDataObject(id));
            WaitHandle.WaitAll(new[] { context.GetWaiter() }, 1000);

            // ASSERT
            Assert.IsNotNull(context.Result);
            Assert.IsTrue(context.Result is PassingCommandDataObject);
            Assert.AreEqual(id, ((PassingCommandDataObject)context.Result).Id);
        }


        [TestMethod]
        public void TwoCommandsTest()
        {
            // ARRANGE
            _сommandDomainInstance.AddContainter(new SimpleContainer());
            var definition1 = CommandBuilder.Build<Command1>().Complete();
            var definition2 = CommandBuilder.Build<Command2>().Complete();
            _сommandDomain.Register(definition1);
            _сommandDomain.Register(definition2);

            // ACT
            var id = Guid.NewGuid();
            var context = _сommandDomainInstance.Start<Command1>(new Command1DataObject());
            WaitHandle.WaitAll(new[] { context.GetWaiter() }, 1000);

            // ASSERT
            Assert.AreEqual(ExecuteStatus.Completed, context.Status);
            Assert.IsNull(context.Result);
            Assert.IsNull(context.Exception);
        }

        [TestMethod]
        public void Test()
        {
            //var result = CommandResultBuilder.NextCommand<Command1>(new Command1DataObject());
            //var command1Result = new Command1Result();
            ////INextCommandResult
            //var commandType = command1Result.GetType();
            //var interfaces = commandType.GetInterfaces();
            //Type d1 = typeof(IDictionary<,>);
            ////Type d2 = typeof(INextCommandResult<,>);
            ////var commadResultInterface = interfaces.FirstOrDefault(t => t == typeof(INextCommandResult<,>));

        }

    }
}
