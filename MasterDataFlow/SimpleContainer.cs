﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Results;

namespace MasterDataFlow
{
    public class SimpleContainer : BaseContainer
    {
        public override void Dispose()
        {
        }

        private BaseCommand _commandInstance;

        protected override void Execute(Guid loopId, ILoopCommandData data, EventLoopCallback callback)
        {
            if (_commandInstance != null)
                throw new Exception("SimpleContainer can handle one command only");
            ThreadPool.QueueUserWorkItem((commandData) =>
            {
                try
                {
                    var commandInfo = (CommandInfo) data;
                    _commandInstance = commandInfo.CommandDefinition.CreateInstance(commandInfo.CommandDataObject);
                    var result = _commandInstance.BaseExecute();
                    callback(loopId, EventLoopCommandStatus.Completed, new ResultCommandMessage(result));
                }
                catch (Exception ex)
                {
                    callback(loopId, EventLoopCommandStatus.Fault, new FaultCommandMessage(ex));
                }
                finally
                {
                    _commandInstance = null;
                }
            });
        }

        protected override void Subscribe(Guid workflowId, TrackedKey key)
        {
            // TODO Call via transaction
            throw new NotImplementedException();
        }

        protected override void Unsubscribe(Guid workflowId, TrackedKey key)
        {
            throw new NotImplementedException();
        }
    }
}
