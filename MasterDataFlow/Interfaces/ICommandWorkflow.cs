using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.EventLoop;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Interfaces
{
    public interface ICommandWorkflow
    {
        WorkflowKey Key { get; }

        CommandDefinition Find<TCommand>()
            where TCommand : ICommand<ICommandDataObject>;

        void Register(CommandDefinition definition);

        void EventLoopCallback(Guid loopId, EventLoopCommandStatus status, ILoopCommandMessage message = null);

        void Subscribe(BaseKey key);

        void Unsubscribe(BaseKey key);
    }
}
