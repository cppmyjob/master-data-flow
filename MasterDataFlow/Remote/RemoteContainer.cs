using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;
using MasterDataFlow.Serialization;

namespace MasterDataFlow.Remote
{
    public class RemoteContainer : BaseContainter
    {
        private readonly IRemoteHostContract _remoteHostContract;

        public RemoteContainer(IRemoteHostContract remoteHostContract)
        {
            _remoteHostContract = remoteHostContract;
        }

        public void Execute(CommandInfo info, OnExecuteContainer onExecute)
        {
            ThreadPool.QueueUserWorkItem((commandData) =>
            {
                //var commandInfo = (CommandInfo)commandData;
                //var commandTypeName = commandInfo.CommandDefinition.Command.AssemblyQualifiedName;
                //var dataObject = Serializator.Serialize(commandInfo.CommandDataObject);
                //string dataObjectTypeName = commandInfo.CommandDataObject.GetType().AssemblyQualifiedName;

                //var requestId = Guid.NewGuid();
                //_remoteHostContract.Execute(requestId, commandInfo.CommandDomainId, commandTypeName, dataObjectTypeName, dataObject);


            }, info);
        }

        public override void Dispose()
        {
            
        }

        public override void Execute(Guid loopId, ILoopCommandData data, EventLoop.EventLoopCallback callback)
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

                    callback(loopId, EventLoopCommandStatus.Completed, null);
                }
                catch (Exception ex)
                {
                    callback(loopId, EventLoopCommandStatus.Fault, new FaultCommandMessage(ex));
                }
            });
        }
    }
}
