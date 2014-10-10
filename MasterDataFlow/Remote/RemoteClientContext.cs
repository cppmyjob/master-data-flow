using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;
using MasterDataFlow.Serialization;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Remote
{
    public abstract class RemoteClientContext : IRemoteClientContext, IRemoteCallback
    {
        private IRemoteHostContract _contract;
        // TODO Remove unnecessary callbacks
        private readonly AsyncDictionary<Guid, EventLoopCallback> _callbacks = new AsyncDictionary<Guid, EventLoopCallback>();


        public IRemoteHostContract Contract
        {
            get
            {
                if (_contract == null)
                {
                    lock (this)
                    {
                        if (_contract == null)
                            _contract = CreateContract();
                    }
                }
                return _contract; 
            }
        }

        protected abstract IRemoteHostContract CreateContract();

        public void RegisterCallback(Guid loopId, EventLoopCallback callback)
        {
            _callbacks.AddItem(loopId, callback);
        }

        public void Callback(string loopId, EventLoopCommandStatus status, string messageTypeName, string messageData)
        {
            var id = new Guid(loopId);
            ILoopCommandMessage message = null;
            if (messageTypeName != null)
            {
                var messageType = Type.GetType(messageTypeName);
                if (messageType == typeof (RemoteDataCommandMessage))
                {
                    // TODO need test for RemoteDataCommandMessage in RemoteClientContextTests
                    var remoteMessage = (RemoteDataCommandMessage) Serializator.Deserialize(messageType, messageData);
                    var data = Serializator.Deserialize(Type.GetType(remoteMessage.DataType), remoteMessage.Data) as ICommandDataObject;
                    message = new DataCommandMessage(data);
                } else
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
            // TODO Restore
            //callback(id, status, message);
        }
    }
}
