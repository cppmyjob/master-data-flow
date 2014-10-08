using System;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Remote;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class RemoteHostControllerTests
    {
        private ManualResetEvent _event;


        public class RemoteHostMock
        {
            private Guid _runLoopId = Guid.Empty;
            private ICommandWorkflow _runWorkflow = null;
            private CommandKey _runCommandKey = null;
            private CommandDefinition _runCommandDefinition = null;
            private ICommandDataObject _runCommandDataObject = null;
            private EventLoopCallback _runCallback = null;
            private int _registerWorkflowCall = 0;
            private WorkflowKey _registerWorkflowKey = null;
            private Mock<IRemoteHost> _host;
            private int _runCall = 0;

            public RemoteHostMock(ICommandWorkflow workflow)
            {
                _host = new Mock<IRemoteHost>();

                _host.Setup(t => t.Run(It.IsAny<Guid>(), It.IsAny<ICommandWorkflow>(), It.IsAny<CommandKey>(), It.IsAny<CommandDefinition>(), It.IsAny<ICommandDataObject>()))
                    .Callback<Guid, ICommandWorkflow, CommandKey, CommandDefinition, ICommandDataObject>(
                    (loopId, workflowParam, commandKey, commandDefinition, commandDataObject) =>
                    {
                        // TODO check _runLoopId in tests
                        _runLoopId = loopId;
                        _runCall = RunCall + 1;
                        _runWorkflow = workflowParam;
                        _runCommandKey = commandKey;
                        _runCommandDefinition = commandDefinition;
                        _runCommandDataObject = commandDataObject;
                    });

                _host.Setup(t => t.RegisterWorkflow(It.IsAny<WorkflowKey>(), It.IsAny<EventLoopCallback>())).Callback<WorkflowKey, EventLoopCallback>((key, callback) =>
                {
                    _registerWorkflowCall = RegisterWorkflowCall + 1;
                    _registerWorkflowKey = key;
                    _runCallback = callback;
                }).Returns(workflow);
            }

            public IRemoteHost Object
            {
                get { return _host.Object; }
            }

            public ICommandWorkflow RunWorkflow
            {
                get { return _runWorkflow; }
            }

            public CommandDefinition RunCommandDefinition
            {
                get { return _runCommandDefinition; }
            }

            public ICommandDataObject RunCommandDataObject
            {
                get { return _runCommandDataObject; }
            }

            public EventLoopCallback RunCallback
            {
                get { return _runCallback; }
            }

            public int RegisterWorkflowCall
            {
                get { return _registerWorkflowCall; }
            }

            public WorkflowKey RegisterWorkflowKey
            {
                get { return _registerWorkflowKey; }
            }

            public int RunCall
            {
                get { return _runCall; }
            }

            public Guid RunLoopId
            {
                get { return _runLoopId; }
            }

            public CommandKey RunCommandKey
            {
                get { return _runCommandKey; }
            }
        }

        public class CommandWorkflowMock
        {
            private int _registerCall = 0;
            private CommandDefinition _registerCommandDefinition = null;
            private readonly Mock<ICommandWorkflow> _workflow = null;

            public CommandWorkflowMock()
            {
                _workflow = new Mock<ICommandWorkflow>();
                _workflow.Setup(t => t.Register(It.IsAny<CommandDefinition>())).Callback<CommandDefinition>(
                    (workflowCommandDefinition) =>
                    {
                        _registerCall = RegisterCall + 1;
                        _registerCommandDefinition = workflowCommandDefinition;
                    });                
            }

            public ICommandWorkflow Object
            {
                get { return _workflow.Object; }
            }

            public int RegisterCall
            {
                get { return _registerCall; }
            }

            public CommandDefinition RegisterCommandDefinition
            {
                get { return _registerCommandDefinition; }
            }
        }

        public class RemoteCallbackMock
        {
            private string _loopId;
            private EventLoopCommandStatus _status;
            private string _messageTypeName;
            private string _messageData;
            private int _call;

            private readonly Mock<IRemoteCallback> _contract;

            public RemoteCallbackMock()
            {
                _contract = new Mock<IRemoteCallback>();
                _contract.Setup(
                    t =>
                        t.Callback(It.IsAny<string>(), It.IsAny<EventLoopCommandStatus>(), It.IsAny<string>(),
                            It.IsAny<string>()))
                    .Callback<string, EventLoopCommandStatus, string, string>(
                        (loopId, status, messageTypeName, messageData) =>
                        {
                            _call = Call + 1;
                            _loopId = loopId;
                            _status = status;
                            _messageTypeName = messageTypeName;
                            _messageData = messageData;
                        });
            }

            public IRemoteCallback Object
            {
                get { return _contract.Object; }
            }

            public string LoopId
            {
                get { return _loopId; }
            }

            public EventLoopCommandStatus Status
            {
                get { return _status; }
            }

            public string MessageTypeName
            {
                get { return _messageTypeName; }
            }

            public string MessageData
            {
                get { return _messageData; }
            }

            public int Call
            {
                get { return _call; }
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _event = new ManualResetEvent(false);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _event.Dispose();
        }

        [TestMethod]
        public void RemoteHostBasicUsageTest()
        {
            // ARRANGE
            var remoteCallback = new RemoteCallbackMock();
            var workflow = new CommandWorkflowMock();
            var host = new RemoteHostMock(workflow.Object);

            var controller = new RemoteHostController(host.Object, remoteCallback.Object);
            var requestId = Guid.NewGuid();
            var workflowKey = new WorkflowKey();
            const string commandTypeName = "MasterDataFlow.Tests.TestData.PassingCommand, MasterDataFlow.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            const string dataObjectTypeName = "MasterDataFlow.Tests.TestData.PassingCommandDataObject, MasterDataFlow.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            const string guid = "1db907fb-77c7-465f-bd60-031107374727";
            const string dataObject = "{\"Id\":\"" + guid + "\"}";
            const string commandId = "8CC9A7EC-AF69-4EBC-BF2C-072E85212BB1";
            var commandKey = new CommandKey(new Guid(commandId));

            // ACT
            controller.Execute(requestId, workflowKey, commandKey, commandTypeName, dataObjectTypeName, dataObject);
            

            // ASSERT
            Assert.AreEqual(1, host.RegisterWorkflowCall);
            Assert.IsNotNull(host.RegisterWorkflowKey);
            Assert.AreEqual(workflowKey, host.RegisterWorkflowKey);

            Assert.AreEqual(1, workflow.RegisterCall);
            Assert.IsNotNull(workflow.RegisterCommandDefinition);
            Assert.AreEqual(commandTypeName, workflow.RegisterCommandDefinition.Command.AssemblyQualifiedName);

            Assert.AreEqual(1, host.RunCall);
            Assert.AreEqual(workflow.Object, host.RunWorkflow);
            Assert.AreEqual(workflow.RegisterCommandDefinition, host.RunCommandDefinition);
            Assert.AreEqual(host.RunCommandKey, commandKey);
            Assert.IsTrue(host.RunCommandDataObject is PassingCommandDataObject);
            Assert.AreEqual(guid, ((PassingCommandDataObject)host.RunCommandDataObject).Id.ToString());

        }

        [TestMethod]
        public void RemoteHostBasicUsageRunCallbackTest()
        {
            // ARRANGE
            var remoteCallback = new RemoteCallbackMock();
            var workflow = new CommandWorkflowMock();
            var host = new RemoteHostMock(workflow.Object);

            var controller = new RemoteHostController(host.Object, remoteCallback.Object);
            var requestId = Guid.NewGuid();
            var workflowKey = new WorkflowKey(); 
            const string commandTypeName = "MasterDataFlow.Tests.TestData.PassingCommand, MasterDataFlow.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            const string dataObjectTypeName = "MasterDataFlow.Tests.TestData.PassingCommandDataObject, MasterDataFlow.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            const string guid = "1db907fb-77c7-465f-bd60-031107374727";
            const string dataObject = "{\"Id\":\"" + guid + "\"}";
            const string commandId = "8CC9A7EC-AF69-4EBC-BF2C-072E85212BB1";
            var commandKey = new CommandKey(new Guid(commandId));

            controller.Execute(requestId, workflowKey, commandKey, commandTypeName, dataObjectTypeName, dataObject);
            // ACT

            Guid callbackGuid = new Guid("33333333-3333-3333-3333-031107374727");
            var callback = host.RunCallback;
            var message = new FaultCommandMessage(new Exception("Test"));
            callback(callbackGuid, EventLoopCommandStatus.Progress, message);

            // ASSERT
            Assert.AreEqual(1, remoteCallback.Call);
            Assert.AreEqual(callbackGuid.ToString(), remoteCallback.LoopId);
            Assert.AreEqual("MasterDataFlow.Messages.FaultCommandMessage, MasterDataFlow, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", 
                remoteCallback.MessageTypeName);
            Assert.AreEqual(
                "{\"Exception\":{\"ClassName\":\"System.Exception\",\"Message\":\"Test\",\"Data\":null,\"InnerException\":null,\"HelpURL\":null,\"StackTraceString\":null,\"RemoteStackTraceString\":null,\"RemoteStackIndex\":0,\"ExceptionMethod\":null,\"HResult\":-2146233088,\"Source\":null,\"WatsonBuckets\":null}}",
                remoteCallback.MessageData);
        }

    }
}
