using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;
using MasterDataFlow.Serialization;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Remote
{
    public class RemoteContainer : BaseContainter, IRemoteCallback
    {
        private readonly IRemoteHostContract _remoteHostContract;
        // TODO Remove unnecessary callbacks
        private readonly AsyncDictionary<Guid, EventLoopCallback> _callbacks = new AsyncDictionary<Guid, EventLoopCallback>();

        public RemoteContainer(IRemoteHostContract remoteHostContract)
        {
            _remoteHostContract = remoteHostContract;
        }

        public override void Dispose()
        {
            
        }

        public override void Execute(Guid loopId, ILoopCommandData data, EventLoopCallback callback)
        {
            ThreadPool.QueueUserWorkItem((commandData) =>
            {
                try
                {
                    var commandInfo = (CommandInfo)data;
                    var commandTypeName = commandInfo.CommandDefinition.Command.AssemblyQualifiedName;
                    var dataObject = Serializator.Serialize(commandInfo.CommandDataObject);
                    string dataObjectTypeName = commandInfo.CommandDataObject.GetType().AssemblyQualifiedName;

                    var requestId = Guid.NewGuid();
                    _remoteHostContract.Execute(requestId, commandInfo.CommandDomain.Id, commandTypeName, dataObjectTypeName, dataObject);
                    callback(loopId, EventLoopCommandStatus.RemoteCall, null);
                    _callbacks.AddItem(loopId, callback);
                }
                catch (Exception ex)
                {
                    callback(loopId, EventLoopCommandStatus.Fault, new FaultCommandMessage(ex));
                }
            });
        }

        public void Callback(string loopId, EventLoopCommandStatus status, string messageTypeName, string messageData)
        {
            var id = new Guid(loopId);
            ILoopCommandMessage message = null;
            if (messageTypeName != null)
            {
                var messageType = Type.GetType(messageTypeName);
                message = Serializator.Deserialize(messageType, messageData) as ILoopCommandMessage;
            }
            var callback = _callbacks.GetItem(id);
            if (callback == null)
            {
                string exMessage = String.Format("RemoteContainer::Callback can't find a callback for loopId {0}", loopId);
                Logger.Instance.Error(exMessage);
                // TODO Change Exception type
                throw new Exception(exMessage);
            }
            callback(id, status, message);
        }
    }
}
