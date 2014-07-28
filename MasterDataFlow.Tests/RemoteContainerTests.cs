using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;
using MasterDataFlow.Remote;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class RemoteContainerTests
    {
        private CommandRunner _runner;
        private ManualResetEvent _event;

        private const string LoopId = "1db907fb-77c7-465f-bd60-031107374727";
        private const string WorkflowId = "C2B980FF-7C4D-4B43-9935-497218492783";

        private class RemoteClientContextMock : RemoteClientContext
        {
            private readonly IRemoteHostContract _contract;

            public RemoteClientContextMock(IRemoteHostContract contract)
            {
                _contract = contract;
            }

            protected override IRemoteHostContract CreateContract()
            {
                return _contract;
            }
        }

        private class RemoteHostContractMock
        {
            private Guid _requestId = System.Guid.Empty;
            private Guid _workflowId = System.Guid.Empty;
            private string _typeName = null;
            private string _dataObject = null;
            private string _dataObjectTypeName = null;
            private int _calls = 0;
            private readonly Mock<IRemoteHostContract> _contract;

            public RemoteHostContractMock()
            {
                _contract = new Mock<IRemoteHostContract>();
                _contract.Setup(t => t.Execute(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).
                    Callback<Guid, Guid, string, string, string>((requestIdParam, workflowIdParam, typeNameParam, dataObjectTypeNameParam, dataObjectParam) =>
                    {
                        _requestId = requestIdParam;
                        _workflowId = workflowIdParam;
                        _typeName = typeNameParam;
                        _dataObjectTypeName = dataObjectTypeNameParam;
                        _dataObject = dataObjectParam;
                        _calls = Calls + 1;
                    });                
            }

            public IRemoteHostContract Object
            {
                get { return _contract.Object; }
            }

            public Guid RequestId
            {
                get { return _requestId; }
            }

            public Guid WorkflowId
            {
                get { return _workflowId; }
            }

            public string TypeName
            {
                get { return _typeName; }
            }

            public string DataObject
            {
                get { return _dataObject; }
            }

            public string DataObjectTypeName
            {
                get { return _dataObjectTypeName; }
            }

            public int Calls
            {
                get { return _calls; }
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _runner = new CommandRunner();
            _event = new ManualResetEvent(false);

        }

        [TestCleanup]
        public void TestCleanup()
        {
            _runner.Dispose();
            _event.Dispose();
        }

        [TestMethod]
        public void ExecuteValidParametersTest()
        {
            // ARRANGE
            var contract = new RemoteHostContractMock();
            var context = new RemoteClientContextMock(contract.Object);
            var container = new RemoteContainer(context);

            var workflow = new CommandWorkflow(new Guid(WorkflowId), _runner);

            var info = new CommandInfo
            {
                CommandDefinition = new CommandDefinition(typeof (PassingCommand)),
                CommandDataObject = new PassingCommandDataObject(new Guid(LoopId)),
                CommandWorkflow = workflow
            };

            // ACT
            container.Execute(System.Guid.NewGuid(), info, (id, status, message) =>
            {
                _event.Set();
            });

            // ASSERT
            _event.WaitOne(1000);
            Assert.AreEqual(1, contract.Calls);
            Assert.AreEqual("MasterDataFlow.Tests.TestData.PassingCommand, MasterDataFlow.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", contract.TypeName);
            Assert.AreEqual("MasterDataFlow.Tests.TestData.PassingCommandDataObject, MasterDataFlow.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", contract.DataObjectTypeName);
            Assert.AreEqual("{\"Id\":\"" + LoopId + "\"}", contract.DataObject);
            Assert.AreEqual(new Guid(WorkflowId), contract.WorkflowId);
            Assert.AreNotEqual(System.Guid.Empty, contract.RequestId);
        }

        [TestMethod]
        public void ExecuteValidCallbackTest()
        {
            // ARRANGE
            var contract = new RemoteHostContractMock();
            var context = new RemoteClientContextMock(contract.Object);
            var container = new RemoteContainer(context);

            var workflow = new CommandWorkflow(new Guid(WorkflowId), _runner);
            var info = new CommandInfo
            {
                CommandDefinition = new CommandDefinition(typeof(PassingCommand)),
                CommandDataObject = new PassingCommandDataObject(new Guid(LoopId)),
                CommandWorkflow = workflow
            };

            // ACT
            Guid? executeId = null;
            var executeStatus = EventLoopCommandStatus.NotStarted;
            ILoopCommandMessage executeMessage = null;
            container.Execute(new Guid(LoopId), info, (id, status, message) =>
            {
                executeId = id;
                executeStatus = status;
                executeMessage = message;
                _event.Set();
            });

            // ASSERT
            _event.WaitOne(1000);
            Assert.AreEqual(new Guid(LoopId), executeId);
            Assert.AreEqual(EventLoopCommandStatus.RemoteCall, executeStatus);
            Assert.IsNull(executeMessage);
        }


        //[TestMethod]
        //public void PassingInputDataToResultTest()
        //{
        //    // ARRANGE
        //    var contract = new RemoteHostContractMock();
        //    var container = new RemoteContainer(contract.Object);
        //    _runner.AddContainter(container);
        //    var commandDefinition = new CommandDefinition(typeof(PassingCommand));

        //    // ACT
        //    var newId = System.Guid.NewGuid();
        //    Guid callbackId = System.Guid.Empty;
        //    var callbackStatus = EventLoopCommandStatus.NotStarted;
        //    ILoopCommandMessage callbackMessage = null;
        //    var originalId = _runner.Run(new CommandDomain(_runner), commandDefinition, new PassingCommandDataObject(newId), (id, status, message) =>
        //    {
        //        callbackId = id;
        //        callbackStatus = status;
        //        callbackMessage = message;
        //        _event.Set();
        //    });

        //    // ASSERT
        //    _event.WaitOne(1000);
        //    Assert.AreEqual(originalId, callbackId);
        //    Assert.AreEqual(EventLoopCommandStatus.Completed, callbackStatus);
        //    Assert.IsNotNull(callbackMessage);
        //    Assert.IsTrue(callbackMessage is DataCommandMessage);
        //    var dataMessage = callbackMessage as DataCommandMessage;
        //    Assert.IsNotNull(dataMessage.Data);
        //    Assert.IsTrue(dataMessage.Data is PassingCommandDataObject);
        //    Assert.AreEqual(newId, ((PassingCommandDataObject)dataMessage.Data).Id);
        //}


    }
}
