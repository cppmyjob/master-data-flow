﻿using System;
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

        public override void Execute(Guid loopId, ILoopCommandData data, EventLoopCallback callback)
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
        }
    }
}
