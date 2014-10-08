using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Serialization;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Remote
{
    public class RemoteContainer : BaseContainer
    {
        private readonly IRemoteClientContext _context;

        public RemoteContainer(IRemoteClientContext remoteHostContract)
        {
            _context = remoteHostContract;
        }

        public override void Dispose()
        {
            // TODO implement
        }

        protected override void Execute(Guid loopId, ILoopCommandData data, EventLoopCallback callback)
        {
            ThreadPool.QueueUserWorkItem((commandData) =>
            {
                try
                {
                    var commandInfo = (CommandInfo)data;
                    var commandTypeName = commandInfo.CommandDefinition.Command.AssemblyQualifiedName;
                    var dataObject = Serializator.Serialize(commandInfo.CommandDataObject);
                    string dataObjectTypeName = commandInfo.CommandDataObject.GetType().AssemblyQualifiedName;

                    //var requestId = Guid.NewGuid();
                    // TODO Try to understand what should i do when exception is thrown during Execute
                    _context.RegisterCallback(loopId, callback);
                    _context.Contract.Execute(loopId, commandInfo.CommandWorkflow.Key, commandInfo.CommandKey, commandTypeName, dataObjectTypeName, dataObject);
                    callback(loopId, EventLoopCommandStatus.RemoteCall, null);
                }
                catch (Exception ex)
                {
                    callback(loopId, EventLoopCommandStatus.Fault, new FaultCommandMessage(ex));
                }
            });
        }

        protected override void Subscribe(WorkflowKey workflowKey, SubscribeKey key)
        {
            throw new NotImplementedException();
        }

        protected override void Unsubscribe(WorkflowKey workflowKey, SubscribeKey key)
        {
            throw new NotImplementedException();
        }
    }
}
