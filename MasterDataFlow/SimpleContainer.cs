using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MasterDataFlow
{
    public class SimpleContainer : BaseContainter
    {
        public override void Execute(CommandInfo info, OnExecuteContainer onExecute)
        {
            ThreadPool.QueueUserWorkItem((commandData) =>
            {
                var commandInfo = (CommandInfo) commandData;
                var commandToExecute = commandInfo.CommandDefinition.CreateInstance(info.CommandDataObject);
                try
                {
                    var result = commandToExecute.BaseExecute();
                    commandInfo.IsError = false;
                    commandInfo.CommandResult = result;
                }
                catch (Exception ex)
                {
                    commandInfo.IsError = true;
                    commandInfo.CommandResult = null;
                }
                finally
                {
                    onExecute(this, commandInfo);
                }
            }, info);
        }

        public override void Dispose()
        {
            
        }
    }
}
