﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;

namespace MasterDataFlow.Interfaces
{
    public interface ICommandWorkflow
    {
        Guid Id { get; }

        CommandDefinition Find<TCommand>()
            where TCommand : ICommand<ICommandDataObject>;

        void Register(CommandDefinition definition);

        void EventLoopCallback(Guid loopId, EventLoopCommandStatus status, ILoopCommandMessage message = null);
    }
}
