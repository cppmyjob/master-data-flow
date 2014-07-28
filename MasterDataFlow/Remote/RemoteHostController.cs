using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Exceptions;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;
using MasterDataFlow.Serialization;

namespace MasterDataFlow.Remote
{
    internal class RemoteHostController : IRemoteHostContract
    {
        private readonly IRemoteCallback _callback;
        private readonly IRemoteHost _remoteHost;

        public RemoteHostController(IRemoteHost remoteHost, IRemoteCallback callback)
        {
            _remoteHost = remoteHost;
            _callback = callback;
        }


        public void UploadAssembly(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void Execute(Guid requestId, Guid workflowId, string commandTypeName, string dataObjectTypeName, string dataObject)
        {
            var workflow = _remoteHost.RegisterWorkflow(workflowId);
            
            var commandType = Type.GetType(commandTypeName);
            var definition = new CommandDefinition(commandType);
            workflow.Register(definition);

            var dataObjectType = Type.GetType(dataObjectTypeName);
            var data = Serializator.Deserialize(dataObjectType, dataObject) as ICommandDataObject;

            _remoteHost.Run(requestId, workflow, definition, data, Callback);
        }

        private void Callback(Guid loopId, EventLoopCommandStatus status, ILoopCommandMessage message)
        {
            string messageTypeName = null;
            string messageData = null;
            if (message != null)
            {
                if (message is DataCommandMessage)
                {
                    var dataMessage = (DataCommandMessage) message;
                    messageTypeName = dataMessage.Data.GetType().AssemblyQualifiedName;
                    messageData = Serializator.Serialize(dataMessage.Data);
                    message = new RemoteDataCommandMessage(messageTypeName, messageData);
                }

                messageTypeName = message.GetType().AssemblyQualifiedName;
                messageData = Serializator.Serialize(message);
            }
            _callback.Callback(loopId.ToString(), status, messageTypeName, messageData);
        }
    }
}
