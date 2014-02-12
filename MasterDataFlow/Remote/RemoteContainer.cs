using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Remote
{
    public class RemoteContainer : BaseContainter
    {
        private readonly IRemoteHostContract _remoteHostContract;

        public RemoteContainer(IRemoteHostContract remoteHostContract)
        {
            _remoteHostContract = remoteHostContract;
        }

        public override void Execute(CommandInfo info, OnExecuteContainer onExecute)
        {
            ThreadPool.QueueUserWorkItem((commandData) =>
            {
                _remoteHostContract.Execute(null, null);
                //var commandInfo = (CommandInfo)commandData;
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
    }
}
