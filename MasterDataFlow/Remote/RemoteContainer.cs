using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Interfaces;
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
                var commandInfo = (CommandInfo)commandData;
                var commandTypeName = commandInfo.CommandDefinition.Command.AssemblyQualifiedName;
                var dataObject = Serializator.Serialize(commandInfo.CommandDataObject);
                string dataObjectTypeName = commandInfo.CommandDataObject.GetType().AssemblyQualifiedName;

                var requestId = Guid.NewGuid();
                //_remoteHostContract.Execute(requestId, commandInfo.CommandDomainId, commandTypeName, dataObjectTypeName, dataObject);

                //var commandToExecute = commandInfo.CommandDefinition.CreateInstance(info.CommandDataObject);
                //try
                //{
                //    var result = commandToExecute.BaseExecute();
                //    commandInfo.IsError = false;
                //    commandInfo.CommandResult = result;
                //}
                //catch (Exception ex)
                //{
                //    commandInfo.IsError = true;
                //    commandInfo.CommandResult = null;
                //}
                //finally
                //{
                //    onExecute(this, commandInfo);
                //}
            }, info);
        }

        public override void Dispose()
        {
            
        }

        public override void Execute(Guid loopId, ILoopCommandData data, EventLoop.EventLoopCallback callback)
        {
            throw new NotImplementedException();
        }
    }
}
