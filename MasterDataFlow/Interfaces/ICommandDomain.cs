using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Interfaces
{
    public interface ICommandDomain
    {
        Guid Id { get; }

        CommandDefinition Find<TCommand>()
            where TCommand : ICommand<ICommandDataObject>;

        void Register(CommandDefinition definition);
    }
}
