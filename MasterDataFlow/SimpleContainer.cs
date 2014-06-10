using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;
using MasterDataFlow.Results;

namespace MasterDataFlow
{
    public class SimpleContainer : BaseContainter
    {
        public override void Dispose()
        {
            
        }

        public override void Execute(Guid loopId, ILoopCommandData data, EventLoopCallback callback)
        {
            ThreadPool.QueueUserWorkItem((commandData) =>
            {
                try
                {
                    var commandInfo = (CommandInfo) data;
                    var commandToExecute = commandInfo.CommandDefinition.CreateInstance(commandInfo.CommandDataObject);
                    var result = commandToExecute.BaseExecute();
                    callback(loopId, EventLoopCommandStatus.Completed, new ResultCommandMessage(result));
                }
                catch (Exception ex)
                {
                    callback(loopId, EventLoopCommandStatus.Fault, new FaultCommandMessage(ex));
                }
            });
        }
    }
}
