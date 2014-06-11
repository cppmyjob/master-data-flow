using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Exceptions;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Serialization;

namespace MasterDataFlow.Remote
{
    internal class RemoteHostController : IRemoteHostContract
    {
        private readonly IRemoteCallback _callback;
        private readonly RemoteHost _remoteHost;

        public RemoteHostController(RemoteHost remoteHost, IRemoteCallback callback)
        {
            _remoteHost = remoteHost;
            _callback = callback;
        }


        public void UploadAssembly(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void Execute(Guid requestId, Guid domainId, string commandTypeName, string dataObjectTypeName, string dataObject)
        {
            var domain = _remoteHost.RegisterDomain(domainId);
            
            var commandType = Type.GetType(commandTypeName);
            var definition = new CommandDefinition(commandType);
            domain.Register(definition);

            var dataObjectType = Type.GetType(dataObjectTypeName);
            var data = Serializator.Deserialize(dataObjectType, dataObject) as ICommandDataObject;

            _remoteHost.Runner.Run(domain, definition, data, Callback);
        }

        private void Callback(Guid loopId, EventLoopCommandStatus status, ILoopCommandMessage message)
        {
            string messageTypeName = null;
            string messageData = null;
            if (message != null)
            {
                messageTypeName = message.GetType().AssemblyQualifiedName;
                messageData = Serializator.Serialize(message);
            }
            _callback.Callback(loopId.ToString(), status, messageTypeName, messageData);
        }
    }
}
