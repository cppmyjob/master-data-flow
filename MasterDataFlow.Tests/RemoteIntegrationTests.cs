using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Actions.UploadType;
using MasterDataFlow.Common.Tests;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Network;
using MasterDataFlow.Network.Packets;
using MasterDataFlow.Tests.TestData;
using MasterDataFlow.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class RemoteIntegrationTests
    {
        private EventWaitHandle _eventWaitHandle;
        private RemoteEnvironment _remote; 

        public class ExecuteCommand : Command<CommandDataObjectStub>
        {

            public override BaseMessage Execute()
            {
                Console.WriteLine("Execute");
                using (var eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset,
                        "AE2E34E2-A03D-4B53-930D-A15CA44BCF21"))
                {
                    eventWaitHandle.Set();
                }
                return Stop(null);
            }

            protected override void OnSubscribed(BaseKey key)
            {
            }

            protected override void OnUnsubscribed(BaseKey key)
            {
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _remote = new RemoteEnvironment(); 
            Logger.SetFactory(new ConsoleLoggerOutputFactory());
            _eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "AE2E34E2-A03D-4B53-930D-A15CA44BCF21");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _remote.Dispose();
            Logger.StopLogging();
            _eventWaitHandle.Close();
        }

        [TestMethod]
        public void RemoteCommandIsExecutedTest()
        {
            // ARRANGE

            // ACT
            _remote.CommandWorkflow.Start<ExecuteCommand>(null);

            // ASSERT
            var result = _eventWaitHandle.WaitOne(2000);
            Assert.IsTrue(result);
        }


        [TestMethod]
        public void RemoteCommandStopEventReturnedTest()
        {
            // ARRANGE
            var callStopCommand = 0;
            BaseMessage callMessage = null;
            _remote.CommandWorkflow.MessageRecieved += (key, message) =>
            {
                ++callStopCommand;
                callMessage = message;
                using (var eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset,
                        "AE2E34E2-A03D-4B53-930D-A15CA44BCF21"))
                {
                    eventWaitHandle.Set();
                }
            };

            // ACT
            var newId = Guid.NewGuid();
            var commandKey = _remote.CommandWorkflow.Start<PassingCommand>(new PassingCommandDataObject(newId));

            // ASSERT
            _eventWaitHandle.WaitOne(2000);

            Assert.AreEqual(1, callStopCommand);
            Assert.IsNotNull(callMessage);
            Assert.IsTrue(callMessage is StopCommandMessage);
            var stopCommand = callMessage as StopCommandMessage;
            Assert.AreEqual(commandKey, stopCommand.Key);

            Assert.IsNotNull(stopCommand.Data);
            var passingCommand = stopCommand.Data as PassingCommandDataObject;
            Assert.IsNotNull(passingCommand);
            Assert.AreEqual(newId, passingCommand.Id);
        }

    }
}
