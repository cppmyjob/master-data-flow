﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Actions;
using MasterDataFlow.Actions.ClientGateKey;
using MasterDataFlow.Actions.UploadType;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;
using MasterDataFlow.Network.Packets;
using MasterDataFlow.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MasterDataFlow.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), TestClass]
    public class ServerGateTests 
    {
        private ManualResetEvent _event;

        [TestInitialize]
        public void TestInitialize()
        {
            _event = new ManualResetEvent(false);

        }

        [TestCleanup]
        public void TestCleanup()
        {
            _event.Close();
        }

        private const string DataObjectId = "1db907fb-77c7-465f-bd60-031107374727";
        private const string WorkflowId = "C2B980FF-7C4D-4B43-9935-497218492783";


        [TestMethod]
        public void ExecuteValidParametersTest()
        {
            // ARRANGE
            var serverMock = new Mock<ServerGate>();
            int calls = 0;
            IPacket recievedPacked = null;
            serverMock.Setup(t => t.Send(It.IsAny<IPacket>())).Callback<IPacket>((packet) =>
            {
                ++calls;
                recievedPacked = packet;
                _event.Set();
            });

            var workflowKey = new WorkflowKey();
            var commandKey = new CommandKey();
            var info = new RemoteExecuteCommandAction.Info
            {
                CommandType = typeof(PassingCommand).AssemblyQualifiedName,
                DataObject = Serialization.Serializator.Serialize(new PassingCommandDataObject(new Guid(DataObjectId))),
                DataObjectType = typeof(PassingCommandDataObject).AssemblyQualifiedName,
                WorkflowKey = workflowKey.Key,
                CommandKey = commandKey.Key
            };
            var bodyObject = new RemoteExecuteCommandAction { CommandInfo = info };

            // ACT
            var senderKey = new ServiceKey();
            var recieverKey = new ServiceKey();

            string bodyTypeName = bodyObject.GetType().AssemblyQualifiedName;
            var body = Serialization.Serializator.Serialize(bodyObject);

            var remotePacket = new RemotePacket(senderKey.Key, recieverKey.Key, bodyTypeName, body);
            serverMock.Object.Send(remotePacket);

            _event.WaitOne(200);

            // ASSERT
            Assert.AreEqual(1, calls);
            Assert.IsNotNull(recievedPacked);
            Assert.IsTrue(recievedPacked.Body is RemoteExecuteCommandAction);
            var recievedInfo = recievedPacked.Body as RemoteExecuteCommandAction;
            Assert.AreEqual(info.CommandKey, recievedInfo.CommandInfo.CommandKey);
            Assert.AreEqual(info.WorkflowKey, recievedInfo.CommandInfo.WorkflowKey);
            Assert.AreEqual(info.DataObjectType, recievedInfo.CommandInfo.DataObjectType);
            Assert.AreEqual(info.DataObject, recievedInfo.CommandInfo.DataObject);
            Assert.AreEqual(info.CommandType, recievedInfo.CommandInfo.CommandType);
        }


        [TestMethod]
        public void LaunchingRemoteCommandRunnerHubTest()
        {
            // ARRANGE
            var commandRunnerMock = new Mock<CommandRunner>() {CallBase = true};
            int calls = 0;
            IPacket recievedPacked = null;
            commandRunnerMock.Setup(t => t.Send(It.IsAny<IPacket>())).Callback<IPacket>((packet) =>
            {
                ++calls;
                recievedPacked = packet;
                _event.Set();
            });

            var serverGate = new ServerGate();
            var commandRunner = commandRunnerMock.Object;
            serverGate.ConnectHub(commandRunner);

            var workflowKey = new WorkflowKey();
            var commandKey = new CommandKey();
            var info = new RemoteExecuteCommandAction.Info
            {
                CommandType = typeof(PassingCommand).AssemblyQualifiedName,
                DataObject = Serialization.Serializator.Serialize(new PassingCommandDataObject(new Guid(DataObjectId))),
                DataObjectType = typeof(PassingCommandDataObject).AssemblyQualifiedName,
                WorkflowKey = workflowKey.Key,
                CommandKey = commandKey.Key
            };
            var bodyObject = new RemoteExecuteCommandAction { CommandInfo = info };

            SendUploadResponse(serverGate, typeof (PassingCommand), workflowKey);
            // TODO Avoid Sleep. It needs for waiting assembly loading
            Thread.Sleep(5000);

            // ACT
            var senderKey = new ServiceKey();
            var recieverKey = serverGate.Key;

            string bodyTypeName = bodyObject.GetType().AssemblyQualifiedName;
            var body = Serialization.Serializator.Serialize(bodyObject);

            var remotePacket = new RemotePacket(senderKey.Key, recieverKey.Key, bodyTypeName, body);
            serverGate.Send(remotePacket);

            _event.WaitOne(20000);

            // ASSERT
            Assert.AreEqual(1, calls);
            Assert.IsNotNull(recievedPacked);
            Assert.IsTrue(recievedPacked.Body is FindContainerAndLaunchCommandAction);
            var recievedCommand = recievedPacked.Body as FindContainerAndLaunchCommandAction;
            Assert.IsNull(recievedCommand.LocalDomainCommandInfo);
            Assert.IsNotNull(recievedCommand.ExternalDomainCommandInfo);
            Assert.AreEqual(commandKey.ToString(), recievedCommand.ExternalDomainCommandInfo.CommandKey);
            Assert.AreEqual(workflowKey, recievedCommand.ExternalDomainCommandInfo.WorkflowKey);

            Assert.AreEqual(typeof(PassingCommand).AssemblyQualifiedName, recievedCommand.ExternalDomainCommandInfo.CommandType);
            Assert.AreEqual(info.DataObject, recievedCommand.ExternalDomainCommandInfo.DataObject);
        }


        //private void SendClientKey()
        //{
        //    var sendClientGateKeyAction = new SendClientGateKeyAction()
        //    {
        //        ClientGateKey = Key.Key
        //    };
        //    var bodyTypeName = sendClientGateKeyAction.GetType().AssemblyQualifiedName;
        //    // TODO need more flexible serialization way
        //    var body = Serialization.Serializator.Serialize(sendClientGateKeyAction);
        //    var remotePacket = new RemotePacket(Key.Key, ServerGateKey.Key, bodyTypeName, body);
        //    _context.Contract.Send(remotePacket);
        //}

        private void SendUploadResponse(IHub hub, Type type, WorkflowKey workflowKey)
        {
            string path = type.Assembly.Location;
            var assemblyFilename = Path.GetFileName(path);
            using (var stream = File.OpenRead(path))
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                var responseAction = new UploadTypeResponseAction
                {
                    TypeName = type.AssemblyQualifiedName,
                    AssemblyData = buffer,
                    AssemblyName = assemblyFilename,
                    WorkflowKey = workflowKey.Key
                };
                hub.Send(new Packet(new ServiceKey(), hub.Key, responseAction));
            }
        }
    }
}
